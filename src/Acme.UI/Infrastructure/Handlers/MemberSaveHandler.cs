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
    public class MemberSaveHandlerComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Components().Append<MemberSaveHandler>();
        }
    }

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
            List<string> processedNodesIds = new List<string>();

            foreach (IMember member in args.SavedEntities)
            {
                if (processedNodesIds.FirstOrDefault(id => id == member.Id.ToString()) == null)
                {
                    if (member.IsApproved && MandatoryFieldsArePopulated(member))
                    {
                        // create tree structure
                        var containerNodeIdAndKey = GetPhysiotherapistsContainerIdAndKey(); // we will (and should) have only one "Physiotherapists" content node to contain all the nodes phyios nodes
                                                                                         // hence no further filtering - only look for the one with "Physiotherapists" alias

                        if (!string.IsNullOrWhiteSpace(containerNodeIdAndKey.Item1))  // Physiotherapists node must exists in Data content. It is the parent of all physios
                        {                                                             // Don't do following if it doesn't
                            
                            var foundNode = findNode(containerNodeIdAndKey.Item1, member.Name);
                            GuidUdi memberDataFolderValue;

                            if (foundNode == null) // if node for new physiotherapist is not already created, then create it and grab its uid
                            {
                                var node = _contentService.Create(member.Name, Guid.Parse(containerNodeIdAndKey.Item2), Published.Physiotherapist.ModelTypeAlias);
                                node.SetValue(GetPhysioPropsAlias(x => x.Physio_name), member.Name);
                                node.SetValue(GetPhysioPropsAlias(x => x.Physio_email), member.Email);

                                var address = GetUmbracoPropertyValue(member.Properties, GetMemberPropsAlias(m => m.Address));
                                var phone = GetUmbracoPropertyValue(member.Properties, GetMemberPropsAlias(m => m.Phone));
                                node.SetValue(GetPhysioPropsAlias(x => x.Physio_address), address);
                                node.SetValue(GetPhysioPropsAlias(x => x.Physio_contact), phone);

                                _contentService.SaveAndPublish(node);

                                processedNodesIds.Add(member.Id.ToString()); // make sure this member is not create again just in case the loop has multiple 
                                                                             // existence for the same member - Note: I found that issue in intial implementation.
                                                                             // I left this logic as a precautionary measure.
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
                            member.SetValue(GetMemberPropsAlias(m => m.MemberDataFolder), memberDataFolderValue);
                        }
                    }
                }
            }
        }
 
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

        private Tuple<string, string> GetPhysiotherapistsContainerIdAndKey()
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
                    return new Tuple<string, string>(node.Id, key);
                }
            }

            return new Tuple<string, string>(null, null);
        }

        private ISearchResult findNode(string parentNodeId, string nodeName)
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
}