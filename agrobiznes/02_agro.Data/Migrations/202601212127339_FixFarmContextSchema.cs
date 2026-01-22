namespace _02_agro.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixFarmContextSchema : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Roslinies", "Row", c => c.Int(nullable: false));
            AddColumn("dbo.Roslinies", "Col", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Roslinies", "Col");
            DropColumn("dbo.Roslinies", "Row");
        }
    }
}
