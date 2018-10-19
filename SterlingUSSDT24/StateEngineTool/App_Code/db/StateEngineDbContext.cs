using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
 
public class StateEngineDbContext : DbContext
{
	public StateEngineDbContext() : base("dbconn")
	{ 
	}
    public DbSet<USSDState> USSDStates { get; set; }
}