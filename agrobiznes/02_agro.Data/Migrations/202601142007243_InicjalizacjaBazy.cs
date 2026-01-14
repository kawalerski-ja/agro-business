namespace _02_agro.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InicjalizacjaBazy : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SystemLogs",
                c => new
                    {
                        LogId = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.LogId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SystemLogs");
        }
    }
}
