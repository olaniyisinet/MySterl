namespace Doubble.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Monday2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ForNonSterlings", "Title", c => c.Int(nullable: false));
            AlterColumn("dbo.ForNonSterlings", "Gender", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ForNonSterlings", "Gender", c => c.String());
            AlterColumn("dbo.ForNonSterlings", "Title", c => c.String());
        }
    }
}
