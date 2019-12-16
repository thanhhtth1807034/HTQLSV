namespace HTQLSV.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitializeDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        RollNumber = c.String(nullable: false, maxLength: 128),
                        FullName = c.String(),
                        Email = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        DeletedAt = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RollNumber);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Students");
        }
    }
}
