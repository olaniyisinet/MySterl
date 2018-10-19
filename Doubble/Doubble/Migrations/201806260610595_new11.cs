namespace Doubble.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class new11 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ForNonSterlings", "DateOfBirth", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ForNonSterlings", "DateOfBirth", c => c.String());
        }
    }
}
