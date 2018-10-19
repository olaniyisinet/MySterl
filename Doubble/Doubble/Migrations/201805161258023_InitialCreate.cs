namespace Doubble.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DoubbleRequests",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        MobileNumber = c.String(),
                        BVN = c.String(),
                        DateOfBirth = c.DateTime(nullable: false),
                        DateOfEntry = c.DateTime(nullable: false),
                        SterlingVerified = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ForNonSterlings",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Gender = c.String(),
                        MobileNumber = c.String(),
                        Email = c.String(),
                        BVN = c.String(),
                        DateOfBirth = c.DateTime(nullable: false),
                        Country = c.String(),
                        State = c.String(),
                        LGA = c.String(),
                        HomeAddress = c.String(),
                        Occupation = c.String(),
                        EmploymentStatus = c.String(),
                        Category = c.String(),
                        IDCard = c.String(),
                        Signature = c.String(),
                        PassPhotograph = c.String(),
                        UtilityBill = c.String(),
                        Amount = c.String(),
                        DateOfEntry = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ForSterlings",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Category = c.String(),
                        PayInAccount = c.String(),
                        BeneficiaryName = c.String(),
                        BeneficiaryAccount = c.String(),
                        DateOfEntry = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ForSterlings");
            DropTable("dbo.ForNonSterlings");
            DropTable("dbo.DoubbleRequests");
        }
    }
}
