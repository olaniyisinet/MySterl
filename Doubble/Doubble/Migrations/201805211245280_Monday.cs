namespace Doubble.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Monday : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.Categories", "Term", c => c.Int(nullable: false));
            AddColumn("dbo.ForSterlings", "Term", c => c.String());
            AddColumn("dbo.ForSterlings", "PayInAmount", c => c.String());
            AddColumn("dbo.ForSterlings", "ReferenceID", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.ForSterlings", "ReferenceID");
            DropColumn("dbo.ForSterlings", "PayInAmount");
            DropColumn("dbo.Categories", "Term");
        }
    }
}
