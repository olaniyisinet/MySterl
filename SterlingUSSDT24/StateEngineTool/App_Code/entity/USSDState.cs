using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

[Table("tbl_USSD_states")]
public class USSDState
{
    [Key, Column(Order = 1)]
    public int op_id { get; set; }
    [Key, Column(Order = 2)]
    public int op_resp { get; set; }
    public string op_desc { get; set; }
    public int next_op_id { get; set; }
    public string op_menu { get; set; }
    public string op_method { get; set; }
    public string op_method_sub { get; set; } 
}