using Umbraco.Core.Composing;
using Umbraco.Core;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Examine;
using Umbraco.Examine;
using System.Linq;
using System;
using System.Collections.Generic;
using Acme.UI.Helper.Extensions;
using Published = Umbraco.Web.PublishedModels;
using System.Linq.Expressions;
using Umbraco.Web;

namespace Acme.UI.Infrastructure.Handlers
{
    public class MemberSaveHandler : IComponent
    {
        private Published.Member _member;
        private Published.Physiotherapist _physio;
        private IUmbracoContextFactory _contextFactory;
        private IContentService _contentService;
        
        public MemberSaveHandler(IContentService contentService, IUmbracoContextFactory contextFactory)
        {
            _member = new Published.Member(null);
            _physio = new Published.Physiotherapist(null);
            _contentService = contentService;
            _contextFactory = contextFactory;
        }

        public void Initialize()
        {
            MemberService.Saving += OnMemberSavingHandler;
        }

        /// <summary>
        /// It is responsible to create newly activated physio's data folder and link it to his/her membership record
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnMemberSavingHandler(IMemberService sender, SaveEventArgs<IMember> args)
        {
            List<string> processedNodesIds = new List<string>(); // To avoid chances to process same node multiple times - noticed it sometimes and could be a possible behaviour of umbraco

            foreach (IMember member in args.SavedEntities)
            {
                if (!processedNodesIds.ContainsExt(member.Id.ToString())) // if not processed already, only then process it
                {
                    if (member.IsApproved && MandatoryFieldsArePopulated(member))
                    {
                        // create tree structure
                        NodeIdAndKey<string, string> containerNodeIdAndKey = GetPhysiotherapistsContainerIdAndKey(); // we will (and should) have only one "Physiotherapists" content node to contain all the nodes phyios nodes
                                                                                                                     // hence no further filtering - only look for the one with "Physiotherapists" alias
                        ProcessMember(containerNodeIdAndKey, member);
                    }
                    processedNodesIds.Add(member.Id.ToString()); // Mark the member as processed - Note: I found that issue in intial implementation.
                                                                 // I left this logic as a precautionary measure.
                }
            }
        }

        /// <summary>
        /// Processes a an member by creating a node for him/her(if it does not exists) and assigning the node reference to his/her membership record
        /// for postlogin redirections to right dashboard
        /// </summary>
        /// <param name="parentNodeIdAndKey"></param>
        /// <param name="memberToProcess"></param>
        private void ProcessMember(NodeIdAndKey<string, string> parentNodeIdAndKey, IMember memberToProcess)
        {
            if (parentNodeIdAndKey != null)  // Physiotherapists node must exists in Data content. It is the parent of all physios
            {                                                             // Don't do following if it doesn't

                var foundNode = FindNode(parentNodeIdAndKey.NodeId, memberToProcess.Username);
                GuidUdi memberDataFolderValue;

                if (foundNode == null) // if node for new physiotherapist is not already created, then create it and grab its uid
                {

                    var node = CreateNode(memberToProcess, parentNodeIdAndKey.NodeKey);

                    memberDataFolderValue = node.GetUdi();

                }
                else // if node for new physiotherapist is already there, then create it and grab its uid anyways
                {
                    // using contextFactory is the recommended way of getting conten with V8 
                    // <see href="https://our.umbraco.com/forum/umbraco-8/96270-using-umbracohelper-in-a-custom-class-in-v8"></see>
                    using (var cref = _contextFactory.EnsureUmbracoContext())
                    {
                        var node = cref.UmbracoContext.Content.GetById(int.Parse(foundNode.Id));
                        memberDataFolderValue = new GuidUdi("document", node.Key);
                    }
                }

                // now set the uid to member's MemberDataFolder property
                memberToProcess.SetValue(GetMemberPropsAlias(m => m.MemberDataFolder), memberDataFolderValue);
            }
        }

        /// <summary>
        /// creates node under the provided parentKey for the member
        /// </summary>
        /// <param name="member"></param>
        /// <param name="parentKey"></param>
        /// <returns></returns>
        private IContent CreateNode(IMember member, string parentKey)
        {
            var node = _contentService.Create(member.Username, Guid.Parse(parentKey), Published.Physiotherapist.ModelTypeAlias);
            node.SetValue(GetPhysioPropsAlias(x => x.Physio_name), member.Name);
            node.SetValue(GetPhysioPropsAlias(x => x.Physio_email), member.Email);

            var address = GetUmbracoPropertyValue(member.Properties, GetMemberPropsAlias(m => m.Address));
            var phone = GetUmbracoPropertyValue(member.Properties, GetMemberPropsAlias(m => m.Phone));
            node.SetValue(GetPhysioPropsAlias(x => x.Physio_address), address);
            node.SetValue(GetPhysioPropsAlias(x => x.Physio_contact), phone);

            _contentService.SaveAndPublish(node);

            return node;
        }

        /// <summary>
        /// validates if all necessary member fields are provided by the user.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private bool MandatoryFieldsArePopulated(IMember member)
        {
            if (string.IsNullOrWhiteSpace(member.Name) || string.IsNullOrWhiteSpace(member.Email))
            {
                return false;
            }

            var address = GetUmbracoPropertyValue(member.Properties, GetMemberPropsAlias(m => m.Address));
            var phone = GetUmbracoPropertyValue(member.Properties, GetMemberPropsAlias(m => m.Phone));

            if (string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(phone))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// The new data node for the incomiong physio will be created under the Data\Physiotherapists node. The function searches 
        /// and returns the Id and Key pair for the the "Physiotherapists" node. The new node will be created under the resulting node(Id and key)
        /// of this function
        /// </summary>
        /// <returns></returns>
        private NodeIdAndKey<string, string> GetPhysiotherapistsContainerIdAndKey()
        {
            if (ExamineManager.Instance.TryGetIndex("ExternalIndex", out var index))
            {
                var searcher = index.GetSearcher();
                var results = searcher.CreateQuery("content").NodeTypeAlias(Published.Physiotherapists.ModelTypeAlias).Execute();

                if (results.Any())
                {
                    var node = results.First();
                    var keyValue = node.Values.FirstOrDefault(x => "__Key".Equals(x.Key));
                    var key = keyValue.Value;
                    return new NodeIdAndKey<string, string>(node.Id, key);
                }
            }

            return null;
        }

        /// <summary>
        /// checks before creating a new node that if it is not already created.
        /// </summary>
        /// <param name="parentNodeId"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        private ISearchResult FindNode(string parentNodeId, string nodeName)
        {
            if (ExamineManager.Instance.TryGetIndex("ExternalIndex", out var index))
            {
                var searcher = index.GetSearcher();
                var results = searcher.CreateQuery("content").ParentId(int.Parse(parentNodeId)).And().NodeName(nodeName).Execute();

                if (results.Any())
                {
                    return results.First();
                }
            }

            return null;
        }

        private string GetUmbracoPropertyValue(PropertyCollection properties, string propertyAlias)
        {
            if (properties.HasValues())
            {
                var property = properties.FirstOrDefault(prop => prop.Alias == propertyAlias);
                if (property != null && property.Values.HasValues())
                {
                    return string.Join(string.Empty, property.Values.Select(value => value.EditedValue != null ? value.EditedValue.ToString() : string.Empty).ToList());
                }
            }

            return null;
        }
 
        public void Terminate()
        {
            MemberService.Saving -= OnMemberSavingHandler; ;
        }

        private string GetMemberPropsAlias<TValue>(Expression<Func<Published.Member, TValue>> selector)
        {
            return _member.GetAlias(Published.Member.GetModelContentType(), selector);
        }

        private string GetPhysioPropsAlias<TValue>(Expression<Func<Published.Physiotherapist, TValue>> selector)
        {
            return _physio.GetAlias(Published.Physiotherapist.GetModelContentType(), selector);
        }

    }

    internal class NodeIdAndKey<TId, TKey>
    {
        public TId NodeId { get; set; }
        public TKey NodeKey { get; set; }

        public NodeIdAndKey(TId id, TKey key)
        {
            NodeId = id;
            NodeKey = key;
        }
    }

}