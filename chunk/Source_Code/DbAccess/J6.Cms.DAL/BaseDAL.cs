﻿using System;
using System.Data;
using System.Text.RegularExpressions;
using J6.Cms.DB;
using J6.Cms.Sql;
using J6.DevFw.Data;

namespace J6.Cms.Dal
{
    public abstract class DalBase
    {
        private static bool _inited = false;

        /// <summary>
        /// 用于生成参数的数据库访问对象
        /// </summary>
        public static IDataBase DbFact;

        private static void CheckAndInit()
        {
            if (!_inited)
            {
                DataBaseAccess _db = CmsDataBase.Instance;
                if (_db == null) throw new ArgumentNullException("_db");
                DbFact = _db.DataBaseAdapter;
                //SQLPack对象
                _inited = true;
            }
        }

        /// <summary>
        /// SQL脚本包对象
        /// </summary>
        protected SqlPack DbSql
        {
            get
            {
                if (_sqlPack == null)
                {
                    DataBaseAccess _db = CmsDataBase.Instance;
                    if (_db == null) throw new ArgumentNullException("_db");
                    _dbType = _db.DbType;
                   _sqlPack=  SqlPack.Factory(_db.DbType);

                }
                return _sqlPack;
            }
        }

       
        /// <summary>
        /// 用于执行操作的数据库访问对象
        /// </summary>
        public DataBaseAccess Db
        {
            get
            {
                return CmsDataBase.Instance;
            }
        }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DataBaseType DbType { get { return _dbType; } }

        /// <summary>
        /// 优化SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        //private static Regex signReg=new Regex("\\$([^\\$]+)\\$");
        private static readonly Regex signReg=new Regex("\\$([^(_|\\s)]+_)");
        private static SqlPack _sqlPack;
        private static DataBaseType _dbType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        protected string OptimizeSql(string sql)
        {
        	if(signReg.IsMatch(sql))
            {
                return signReg.Replace(sql, m =>
                {
                    switch (m.Groups[1].Value.ToUpper())
                    {
                        case "PREFIX_": return CmsDataBase.TablePrefix;
                        /*
                        case "SITE":
                            if(j6.MultSiteVersion)
                            {
                                Site site = Cms.Context.CurrentSite;
                                return " siteid=" + site.SiteId.ToString();
                            }
                            return null;

                        case "ANDSITE":
                            if(j6.MultSiteVersion)
                            {
                                Site site = Cms.Context.CurrentSite;
                                return " AND siteid=" + site.SiteId.ToString();
                            }
                            return null;
                        */
                    }
                    return string.Empty;
                });
            }
            return sql;
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        protected int ExecuteNonQuery(params SqlQuery[] sql)
        {
            int result=this.Db.ExecuteNonQuery(sql);
            //db.CloseConn();
            return result;
        }

        protected object ExecuteScalar(SqlQuery sqlEnt)
        {
            object result = Db.ExecuteScalar(sqlEnt);
            //db.CloseConn();
            return result;
        }


        protected void ExecuteReader(SqlQuery sql, DataReaderFunc func)
        {
            this.Db.ExecuteReader(sql, func);
           // db.CloseConn();
        }

        public DataSet GetDataSet(SqlQuery sqlEnt)
        {
            DataSet ds = this.Db.GetDataSet(sqlEnt);
            //db.CloseConn();
            return ds;

        }
    }
}