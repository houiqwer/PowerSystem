using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerSystemLibrary.Entity;
using PowerSystemLibrary.Util;
using PowerSystemLibrary.Enum;


namespace PowerSystemLibrary.DBContext
{

    public class PowerSystemDBContext : DbContext
    {
        public PowerSystemDBContext()
            : base(ConfigurationManager.ConnectionStrings["PowerSystem"].ConnectionString)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<PowerSystemDBContext, Configuration<PowerSystemDBContext>>());
        }

        public PowerSystemDBContext(string dbpath)
            : base(dbpath)
        {
            //根据代码修改库
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<PowerSystemDBContext, Configuration<PowerSystemDBContext>>());
        }

        public DbSet<Log> Log { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Menu> Menu { get; set; }
        public DbSet<ElectricalTask> ElectricalTask { get; set; }
        public DbSet<ElectricalTaskUser> ElectricalTaskUser { get; set; }
        public DbSet<AH> AH { get; set; }
        public DbSet<ApplicationSheet> ApplicationSheet { get; set; }
        public DbSet<Operation> Operation { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    public class Configuration<T> : DbMigrationsConfiguration<T> where T : DbContext
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true; //任何Model Class的修改將會直接更新DB
            AutomaticMigrationDataLossAllowed = true;
        }
        /// <summary>
        /// 初始化基本数据
        /// </summary>
        /// <param name="context"></param>
        protected override void Seed(T context)
        {
            base.Seed(context);
            Init(context);
        }

        public void Init(T context)
        {

            //SysUser user = context.Set<SysUser>().FirstOrDefault(t => t.UserName == "admin");
            //if (user == null)
            //{
            //    context.Set<SysUser>().AddOrUpdate(t => t.UserName, new SysUser()
            //    {
            //        UserName = "admin",
            //        RealName = "超级管理员",
            //        Password = new BaseUtil().BuildPassword("admin", "yzy@2021"),
            //        CellPhone = "18888888888",
            //        Post = "超管",
            //        DepartmentID = department.ID
            //    });
            //    context.SaveChanges();
            //    user = context.Set<SysUser>().FirstOrDefault(t => t.UserName == "admin");
            //}



        }
    }
}
