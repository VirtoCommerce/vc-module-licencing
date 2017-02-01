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
                        CustomerEmail = c.String(nullable: false, maxLength: 256),
                        CustomerName = c.String(nullable: false, maxLength: 256),
                        ActivationCode = c.String(nullable: false, maxLength: 16),
                        Signature = c.String(nullable: false, maxLength: 64),
                        ExpirationDate = c.DateTime(nullable: false),
                        Type = c.String(nullable: false, maxLength: 32),
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
