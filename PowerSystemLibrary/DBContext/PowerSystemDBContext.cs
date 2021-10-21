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
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Menu> Menu { get; set; }
        public DbSet<ElectricalTask> ElectricalTask { get; set; }
        public DbSet<ElectricalTaskUser> ElectricalTaskUser { get; set; }
        public DbSet<AH> AH { get; set; }
        public DbSet<ApplicationSheet> ApplicationSheet { get; set; }
        public DbSet<Operation> Operation { get; set; }
        public DbSet<AuditProcess> AuditProcess { get; set; }
        public DbSet<PowerSubstation> PowerSubstation { get; set; }
        public DbSet<Role_Right> Role_Right { get; set; }

        public DbSet<WorkSheet> WorkSheet { get; set; }
        public DbSet<OperationSheet> OperationSheet { get; set; }
        public DbSet<SendElectricalSheet> SendElectricalSheet { get; set; }
        public DbSet<OperationContent> OperationContent { get; set; }
        public DbSet<OperationSheet_Content> OperationSheet_Content { get; set; }

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
            Department department = context.Set<Department>().FirstOrDefault();
            if (department == null)
            {
                context.Set<Department>().AddOrUpdate(t => t.Name, new Department()
                {
                    Name = "安徽港口集团芜湖有限公司",
                    No = "1",
                });
                context.SaveChanges();
                department = context.Set<Department>().FirstOrDefault(t => t.Name == "安徽港口集团芜湖有限公司");
            }

            User user = context.Set<User>().FirstOrDefault(t => t.Username == "admin");
            if (user == null)
            {
                context.Set<User>().AddOrUpdate(t => t.Username, new User()
                {
                    Username = "admin",
                    Realname = "超级管理员",
                    Password = new BaseUtil().BuildPassword("admin", "admin"),
                    Cellphone = "18888888888",
                    DepartmentID = department.ID
                });
                context.SaveChanges();
                user = context.Set<User>().FirstOrDefault(t => t.Username == "admin");

                context.Set<UserRole>().AddOrUpdate(t => t.UserID, new UserRole()
                {
                    UserID = user.ID,
                    Role = Role.系统管理员
                });
                context.SaveChanges();

            }

            PowerSubstation powerSubstation = context.Set<PowerSubstation>().FirstOrDefault();
            if (powerSubstation == null)
            {
                context.Set<PowerSubstation>().AddOrUpdate(t => t.Name, new PowerSubstation()
                {
                    Name = "4#变电所",
                    
                });
                context.Set<PowerSubstation>().AddOrUpdate(t => t.Name, new PowerSubstation()
                {
                    Name = "5#变电所",

                });
                context.SaveChanges();
            }


        }
    }
}
