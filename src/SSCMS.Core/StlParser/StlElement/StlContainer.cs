﻿using System.Text;
using System.Threading.Tasks;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Parse;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "容器", Description = "通过 stl:container 标签在模板中定义容器，切换上下文")]
    public static class StlContainer
    {
        public const string ElementName = "stl:container";

        [StlAttribute(Title = "栏目索引")]
        private const string ChannelIndex = nameof(ChannelIndex);

        [StlAttribute(Title = "栏目索引")]
        private const string Index = nameof(Index);

        [StlAttribute(Title = "栏目名称")]
        private const string ChannelName = nameof(ChannelName);

        [StlAttribute(Title = "父栏目")]
        private const string Parent = nameof(Parent);

        [StlAttribute(Title = "上级栏目的级别")]
        private const string UpLevel = nameof(UpLevel);

        [StlAttribute(Title = "从首页向下的栏目级别")]
        private const string TopLevel = nameof(TopLevel);

        [StlAttribute(Title = "所处上下文")]
        private const string Context = nameof(Context);

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var contextInfo = parseManager.ContextInfo;

            if (contextInfo.IsStlEntity || string.IsNullOrWhiteSpace(contextInfo.InnerHtml)) return string.Empty;

            var channelIndex = string.Empty;
            var channelName = string.Empty;
            var upLevel = 0;
            var topLevel = -1;
            var context = contextInfo.ContextType;

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, ChannelIndex) || StringUtils.EqualsIgnoreCase(name, Index))
                {
                    channelIndex = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelName))
                {
                    channelName = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Parent))
                {
                    if (TranslateUtils.ToBool(value))
                    {
                        upLevel = 1;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, UpLevel))
                {
                    upLevel = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, TopLevel))
                {
                    topLevel = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Context))
                {
                    context = TranslateUtils.ToEnum(value, ParseType.Undefined);
                }
            }

            var prevContextType = contextInfo.ContextType;
            var prevChannelId = contextInfo.ChannelId;

            var dataManager = new StlDataManager(parseManager.DatabaseManager);
            var channelId = await dataManager.GetChannelIdByLevelAsync(parseManager.PageInfo.SiteId, contextInfo.ChannelId, upLevel, topLevel);

            channelId = await dataManager.GetChannelIdByChannelIdOrChannelIndexOrChannelNameAsync(parseManager.PageInfo.SiteId, channelId, channelIndex, channelName);

            contextInfo.ContextType = context;
            contextInfo.ChannelId = channelId;

            var innerHtml = RegexUtils.GetInnerContent(ElementName, contextInfo.OuterHtml);

            var builder = new StringBuilder(innerHtml);
            await parseManager.ParseInnerContentAsync(builder);

            contextInfo.ContextType = prevContextType;
            contextInfo.ChannelId = prevChannelId;

            return builder.ToString();
        }
    }
}
