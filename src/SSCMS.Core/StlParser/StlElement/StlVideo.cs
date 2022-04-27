﻿using System.Threading.Tasks;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Parse;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;
using System.Collections.Specialized;
using SSCMS.Enums;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "播放视频", Description = "通过 stl:video 标签在模板中显示视频播放器")]
    public static class StlVideo
    {
        public const string ElementName = "stl:video";

        [StlAttribute(Title = "指定视频的字段")]
        private const string Type = nameof(Type);

        [StlAttribute(Title = "视频地址")]
        private const string PlayUrl = nameof(PlayUrl);

        [StlAttribute(Title = "图片地址")]
        private const string ImageUrl = nameof(ImageUrl);

        [StlAttribute(Title = "宽度")]
        private const string Width = nameof(Width);

        [StlAttribute(Title = "高度")]
        private const string Height = nameof(Height);

        [StlAttribute(Title = "是否自动播放")]
        private const string IsAutoPlay = nameof(IsAutoPlay);

        [StlAttribute(Title = "是否显示播放控件")]
        private const string IsControls = nameof(IsControls);

        [StlAttribute(Title = "是否循环播放")]
        private const string IsLoop = nameof(IsLoop);

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var type = nameof(Content.VideoUrl);
            var playUrl = string.Empty;
            var imageUrl = string.Empty;
            var width = string.Empty;
            var height = string.Empty;
            var isAutoPlay = false;
            var isControls = true;
            var isLoop = false;
            var attributes = new NameValueCollection();

            foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, PlayUrl))
                {
                    playUrl = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, ImageUrl))
                {
                    imageUrl = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Width))
                {
                    width = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Height))
                {
                    height = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsAutoPlay))
                {
                    isAutoPlay = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsControls))
                {
                    isControls = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsLoop))
                {
                    isLoop = TranslateUtils.ToBool(value, false);
                }
                else
                {
                    attributes[name] = value;
                }
            }

            return await ParseAsync(parseManager, type, playUrl, imageUrl, width, height, isAutoPlay, isControls, isLoop, attributes);
        }

        private static async Task<string> ParseAsync(IParseManager parseManager, string type, string playUrl, string imageUrl, string width, string height, bool isAutoPlay, bool isControls, bool isLoop, NameValueCollection attributes)
        {
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            var videoUrl = string.Empty;
            if (!string.IsNullOrEmpty(playUrl))
            {
                videoUrl = playUrl;
            }
            else
            {
                var contentId = contextInfo.ContentId;
                if (contextInfo.ContextType == ParseType.Content)
                {
                    if (contentId != 0)//获取内容视频
                    {
                        var contentInfo = await parseManager.GetContentAsync();
                        if (contentInfo != null)
                        {
                            videoUrl = contentInfo.Get<string>(type);
                            if (string.IsNullOrEmpty(videoUrl))
                            {
                                videoUrl = contentInfo.VideoUrl;
                            }
                        }
                    }
                }
                else if (contextInfo.ContextType == ParseType.Each)
                {
                    videoUrl = contextInfo.ItemContainer.EachItem.Value as string;
                }
            }

            if (string.IsNullOrEmpty(imageUrl))
            {
                var contentId = contextInfo.ContentId;
                if (contextInfo.ContextType == ParseType.Content)
                {
                    if (contentId != 0)//获取内容视频
                    {
                        var contentInfo = await parseManager.GetContentAsync();
                        if (contentInfo != null)
                        {
                            imageUrl = contentInfo.ImageUrl;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(videoUrl)) return string.Empty;

            videoUrl = await parseManager.PathManager.ParseSiteUrlAsync(pageInfo.Site, videoUrl, pageInfo.IsLocal);
            imageUrl = await parseManager.PathManager.ParseSiteUrlAsync(pageInfo.Site, imageUrl, pageInfo.IsLocal);

            await pageInfo.AddPageBodyCodeIfNotExistsAsync(ParsePage.Const.JsAcVideoJs);

            attributes["class"] = "video-js vjs-big-play-centered" + (string.IsNullOrEmpty(attributes["class"]) ? string.Empty : " " + attributes["class"]);

            var innerHtml = string.Empty;
            if (FileUtils.IsType(FileType.Mp4, PageUtils.GetExtensionFromUrl(videoUrl)))
            {
                innerHtml = @$"<source src=""{videoUrl}"" type=""video/mp4"" />";
            }
            else if (FileUtils.IsType(FileType.Webm, PageUtils.GetExtensionFromUrl(videoUrl)))
            {
                innerHtml = @$"<source src=""{videoUrl}"" type=""video/webm"" />";
            }
            else
            {
                attributes["src"] = videoUrl;
            }

            if (isAutoPlay)
            {
                attributes["autoplay"] = "true";
            }
            if (isControls)
            {
                attributes["controls"] = "true";
            }
            if (isLoop)
            {
                attributes["loop"] = "true";
            }
            if (!string.IsNullOrEmpty(imageUrl))
            {
                attributes["poster"] = imageUrl;
            }

            var styles = ".vjs-poster { background-size: cover !important; }";
            if (!string.IsNullOrEmpty(width))
            {
                attributes["width"] = width;
            }
            else
            {
                styles += ".video-js {min-width: 100%;width: auto;}";
            }

            attributes["height"] = string.IsNullOrEmpty(height) ? "500" : height;
            attributes["data-setup"] = "{}";

            return $@"
            <style>{styles}</style>
            <video {TranslateUtils.ToAttributesString(attributes)}>{innerHtml}</video>
";
        }
    }
}
