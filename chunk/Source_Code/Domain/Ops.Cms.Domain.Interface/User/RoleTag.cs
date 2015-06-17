﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtNet.Cms.Domain.Interface.User
{
    [Flags]
    /// <summary>
    /// 角色标签
    /// </summary>
    public enum RoleTag:int
    {
        /// <summary>
        /// 内容发布者
        /// </summary>
        Publisher = 1 << 0,

        //todo: add other

        /// <summary>
        /// 站点管理员
        /// </summary>
        SiteOwner = 1 << 8,

        /// <summary>
        /// 超级管理员
        /// </summary>
        Master = 1 << 9
    }
}
