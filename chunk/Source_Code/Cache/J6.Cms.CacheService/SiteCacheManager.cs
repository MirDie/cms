﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using J6.Cms.Cache;
using J6.Cms.Cache.CacheCompoment;
using J6.Cms.DataTransfer;

namespace J6.Cms.CacheService
{
    public static class SiteCacheManager 
    {
        private static SiteDto _defaultSite;
        /// <summary>
        /// 获取所有的站点
        /// </summary>
        /// <returns></returns>
        public static IList<SiteDto> GetAllSites()
        {
            return ServiceCall.Instance.SiteService.GetSites();
        }

        /// <summary>
        /// 获取站点
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public static SiteDto GetSite(int siteId)
        {
            //foreach (SiteDto s in WeakRefCache.Sites)
            //{
            //    if (s.SiteId == siteID) return s;
            //}
            //return null;

            return ServiceCall.Instance.SiteService.GetSiteById(siteId);
        }

        public static SiteDto GetSingleOrDefaultSite(Uri uri)
        {  
            string siteCacheKey=null;
            string hostName = uri.Host;
            string appDirName = uri.Segments.Length==1?
                null:
                uri.Segments[1].Replace("/","");

            if(appDirName!=null){
                foreach(SiteDto site in WeakRefCache.Sites){
                    if(String.Compare(site.DirName,appDirName,true,CultureInfo.InvariantCulture)==0){
                        siteCacheKey=String.Concat(CacheSign.Site.ToString(), "_host_", hostName,"_",appDirName);
                        break;
                    }
                }
            }

            if(siteCacheKey==null){
                siteCacheKey=String.Concat(CacheSign.Site.ToString(), "_host_", hostName);
            }



            return CacheFactory.Sington.GetCachedResult<SiteDto>(siteCacheKey,
                 () =>
                 {
                     SiteDto site = ServiceCall.Instance.SiteService.GetSingleOrDefaultSite(uri);
                     SiteDto dto = GetSite(site.SiteId);
                     dto.RunType = site.RunType;
                     return site;
                 },DateTime.Now.AddHours(24));
        }

        /// <summary>
        /// 默认站点(暂管理员使用)
        /// </summary>
        public static SiteDto DefaultSite
        {
            get
            {
                if (_defaultSite.SiteId<=0)
                {
                    IList<SiteDto> sites = ServiceCall.Instance.SiteService.GetSites();
                    if (sites.Count == 0)
                    {
                        throw new ArgumentException("没有可用的站点!");
                    }

                    //如果找不到站点，则获取默认第一个站点
                    if (_defaultSite.SiteId <= 0)
                    {
                        _defaultSite = sites[0];
                    }
                }
                return _defaultSite;
            }
        }
    }
}
