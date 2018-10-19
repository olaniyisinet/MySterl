namespace Doubble.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Wednesday : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ForSterlings", "BeneficiaryType", c => c.String());
            DropColumn("dbo.ForSterlings", "PayInType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ForSterlings", "PayInType", c => c.String());
            DropColumn("dbo.ForSterlings", "BeneficiaryType");
        }
    }
}
