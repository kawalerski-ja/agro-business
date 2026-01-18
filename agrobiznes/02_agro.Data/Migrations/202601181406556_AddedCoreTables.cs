namespace _02_agro.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCoreTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Devices",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        IsOn = c.Boolean(nullable: false),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Roslinies",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Nazwa = c.String(),
                        TypRosliny1 = c.Int(nullable: false),
                        Poziom_wzrostu = c.Single(nullable: false),
                        Poziom_nawodnienia = c.Single(nullable: false),
                        Poziom_naslonecznienia = c.Single(nullable: false),
                        Cena = c.Single(nullable: false),
                        Wzrost_na_tick = c.Single(nullable: false),
                        Odwodnienie_na_tick = c.Single(nullable: false),
                        Zuzycie_energii_slonecznej_na_tick = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Roslinies");
            DropTable("dbo.Devices");
        }
    }
}
