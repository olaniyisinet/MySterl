namespace Doubble.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _new : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ForSterlings", "SelfOrOthers", c => c.String());
            DropColumn("dbo.ForSterlings", "Self");
            DropColumn("dbo.ForSterlings", "Others");
            DropColumn("dbo.ForSterlings", "OthersPar");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ForSterlings", "OthersPar", c => c.String());
            AddColumn("dbo.ForSterlings", "Others", c => c.String());
            AddColumn("dbo.ForSterlings", "Self", c => c.String());
            DropColumn("dbo.ForSterlings", "SelfOrOthers");
        }
    }
}
