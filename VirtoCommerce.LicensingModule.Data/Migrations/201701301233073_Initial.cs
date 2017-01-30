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
                        CustomerEmail = c.String(),
                        CustomerName = c.String(),
                        PublicKey = c.String(),
                        Signature = c.String(),
                        ExpirationDate = c.DateTime(nullable: false),
                        Type = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedBy = c.String(maxLength: 64),
                        ModifiedBy = c.String(maxLength: 64),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.License");
        }
    }
}
