namespace VirtoCommerce.LicensingModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.License",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Type = c.String(nullable: false, maxLength: 64),
                        CustomerName = c.String(nullable: false, maxLength: 256),
                        CustomerEmail = c.String(nullable: false, maxLength: 256),
                        ExpirationDate = c.DateTime(nullable: false),
                        ActivationCode = c.String(nullable: false, maxLength: 64),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedBy = c.String(maxLength: 64),
                        ModifiedBy = c.String(maxLength: 64),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ActivationCode, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.License", new[] { "ActivationCode" });
            DropTable("dbo.License");
        }
    }
}
