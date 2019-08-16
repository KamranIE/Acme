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
        private const string memberDataFolderAlias = "memberDataFolder";
        private const string addressAlias = "address";
        private const string phoneAlias = "phone";

        private IContentService _contentService;
        
        public MemberSaveHandler(IContentService contentService)
        {
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
                        var parentNodeIdAndKey = GetParentNodeIdAndKey();
                        if (!string.IsNullOrWhiteSpace(parentNodeIdAndKey.Item1) && !nodeExists(parentNodeIdAndKey.Item1, member.Name))
                        {
                            var node = _contentService.Create(member.Name, Guid.Parse(parentNodeIdAndKey.Item2), "physiotherapist");
                            node.SetValue("physio_name", member.Name);
                            node.SetValue("physio_email", member.Email);

                            var address = GetUmbracoPropertyValue(member.Properties, addressAlias);
                            var phone = GetUmbracoPropertyValue(member.Properties, phoneAlias);
                            node.SetValue("physio_address", address);
                            node.SetValue("physio_contact", phone);

                            _contentService.SaveAndPublish(node);
                            processedNodesIds.Add(member.Id.ToString());

                            member.SetValue(memberDataFolderAlias, node.GetUdi());
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

            var address = GetUmbracoPropertyValue(member.Properties, addressAlias);
            var phone = GetUmbracoPropertyValue(member.Properties, phoneAlias);

            if (string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(phone))
            {
                return false;
            }

            return true;
        } 

        private Tuple<string, string> GetParentNodeIdAndKey()
        {
            if (ExamineManager.Instance.TryGetIndex("ExternalIndex", out var index))
            {
                var searcher = index.GetSearcher();
                var results = searcher.CreateQuery("content").NodeTypeAlias("physiotherapists").Execute();

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
    }
}