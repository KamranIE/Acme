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

        private IContentService _contentService;
        
        public MemberSaveHandler(IContentService contentService)
        {
            _member = new Published.Member(null);
            _physio = new Published.Physiotherapist(null);
            _contentService = contentService;
        }

        public void Initialize()
        {
            MemberService.Saving += OnMemberSavingHandler;
        }
        
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

                        if (!string.IsNullOrWhiteSpace(containerNodeIdAndKey.Item1) && !nodeExists(containerNodeIdAndKey.Item1, member.Name)) // make sure the new member does not already exist under "Physiotherapists"
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

                            member.SetValue(GetMemberPropsAlias(m => m.MemberDataFolder), node.GetUdi());
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

        private bool nodeExists(string parentNodeId, string nodeName)
        {
            if (ExamineManager.Instance.TryGetIndex("ExternalIndex", out var index))
            {
                var searcher = index.GetSearcher();
                var results = searcher.CreateQuery("content").ParentId(int.Parse(parentNodeId)).And().NodeName(nodeName).Execute();

                if (results.Any())
                {
                    return true;
                }
            }

            return false;
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