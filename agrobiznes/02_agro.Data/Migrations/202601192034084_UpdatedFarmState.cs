namespace _02_agro.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedFarmState : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Devices", "Cena", c => c.Single(nullable: false));
            AddColumn("dbo.Roslinies", "CenaSprzedazy", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Roslinies", "CenaSprzedazy");
            DropColumn("dbo.Devices", "Cena");
        }
    }
}
