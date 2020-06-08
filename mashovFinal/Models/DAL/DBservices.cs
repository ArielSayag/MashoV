using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;
using System.Text;
using mashovFinal.Models;
using static mashovFinal.Utilities.StringHelper;
using static mashovFinal.Utilities.MailHelper;
using static mashovFinal.Utilities.CritRanHelper;

/// <summary>
/// DBServices is a class created by me to provides some DataBase Services
/// </summary>
public class DBservices
{
    public SqlDataAdapter da;
    public DataTable dt;
    public DataTableCollection dt1;

    public DBservices()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    //--------------------------------------------------------------------------------------------------
    // This method creates a connection to the database according to the connectionString name in the web.config 
    //--------------------------------------------------------------------------------------------------
    public SqlConnection connect(String conString)
    {

        // read the connection string from the configuration file
        string cStr = WebConfigurationManager.ConnectionStrings[conString].ConnectionString;
        SqlConnection con = new SqlConnection(cStr);
        con.Open();
        return con;
    }
    //---------------------------------------------------------------------------------
    // Create the SqlCommand
    //---------------------------------------------------------------------------------
    private SqlCommand CreateCommand(String CommandSTR, SqlConnection con)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = CommandSTR;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.Text; // the type of the command, can also be stored procedure

        return cmd;
    }
    //--------------------------------------------------------------------------------------------------
    // This method if Doc not ready on time  CLASSEX
    //--------------------------------------------------------------------------------------------------
    public List<Users> findMaxManager(int numDoc)
    {

        List<Users> allmanager = new List<Users>();
       
        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = @"select u.* from Users u inner join Manager_feedback m on u.Email=m.EmailManager
                                inner join FeedBack_Doc d  on m.NumDoc=d.NumDoc
                                 where d.NumDoc='"+numDoc+"'";


            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {

                Users u = new Users();
                u.Email= (string)dr["Email"];
                u.FirstName = (string)dr["FirstName"];
                u.LastName = (string)dr["LastName"];


                allmanager.Add(u);
            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return allmanager;
    }
    public int checkedDoc()
    {
        List<CritInDoc> allD = new List<CritInDoc>();
        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = @"select m.nameMetting,m.DateMeeting,d.totalW ,d.[status],m.NumDepartment,m.NumCourse ,d.NumDoc
                                 from FeedBack_Doc d inner join FeedBack_Meet m on d.NumMeeting=m.NumMeeting
                                 where CONVERT(varchar(20), m.DateMeeting,110) <=CONVERT(VARCHAR(20), GETDATE(), 101)
                                 and d.totalW<100";


            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                CritInDoc cd = new CritInDoc();
                cd.Doc = new FeedBack_Doc();
                cd.Doc.DetailsMeet = new FeedBack_Meeting();
                cd.Doc.DetailsMeet.DetailsCourseDep = new CoursesAndDepartment();

                cd.Doc.NumDoc = Convert.ToInt32(dr["NumDoc"]);
                cd.Doc.TotalWeight = Convert.ToDouble(dr["totalW"]);
                cd.Doc.NameDoc = (string)dr["nameMetting"];
                cd.Doc.DetailsMeet.Date = (string)dr["DateMeeting"];
                cd.Doc.DetailsMeet.DetailsCourseDep.NumCourse = Convert.ToInt32(dr["NumCourse"]);
                cd.Doc.DetailsMeet.DetailsCourseDep.NumDepartment = Convert.ToInt32(dr["NumDepartment"]);
                cd.Doc.Status = Convert.ToBoolean(dr["status"]);

                allD.Add(cd);
            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        int num = 0;
        if (allD.Count > 0)
        {
            foreach (var item in allD)
            {
                num += allScoreGroup(item);
                
              
            }

        }

        return num;
    }
    public int allScoreGroup(CritInDoc d)
    {

        List<Criterion> allC = new List<Criterion>();

        SqlConnection con = null;

        CoursesAndDepartment cd = d.Doc.DetailsMeet.DetailsCourseDep;


        try
        {
            con = connect("con15");
            String selectSTR = @"select c.* , MAX(ca.WeightCrit) as 'WeightCrit' , MAX(ca.numScala) as 'numScala'
                               from FeedBack_Doc d inner join FeedBack_Criteria ca on d.NumDoc=ca.NumDoc
                               inner join FeedBack_Meet m on m.NumMeeting=d.NumMeeting inner join Criteria c on c.NumCrit=ca.NumCrit
                               where m.NumDepartment='"+ cd.NumDepartment + "' and m.NumCourse='" + cd.NumCourse + "' "+
                               "and ca.NumCrit not in ( select NumCrit  from FeedBack_Criteria where NumDoc ='" + d.Doc.NumDoc + "') "+
                               "group by c.NumCrit, c.NameCrit, c.DescriptionCrit , c.counterScore";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                Criterion c = new Criterion();

                c.NumCrit = Convert.ToInt32(dr["NumCrit"]);
                c.NameCrit = (string)dr["NameCrit"];
                c.DescriptionCrit = (string)dr["DescriptionCrit"];
                c.WeightCrit = Convert.ToDouble(dr["WeightCrit"]);
                c.TypeCrit = Convert.ToInt32(dr["numScala"]);
                c.Favorite = Convert.ToInt32(dr["counterScore"]);
                allC.Add(c);
            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        if (allC.Count > 0)
        {
            d.AllCrit = createNewListCrit(allC, d.Doc.TotalWeight);//(bank crit,total);
            d.Doc.TotalWeight = 100;

          List<Criterion> c1= insert(d, true);//BuildInsertCommand
          
            List<Users> managerFeed = findMaxManager(d.Doc.NumDoc);
            foreach (var manager in managerFeed)
            {
                string subj = "התבצע עדכון משוב אותו אתה מנהל";
                string body = @"שלום " + manager.FirstName + " " + manager.LastName + "<br/><br/>" +
                                "בתאריך:  " + d.Doc.DetailsMeet.Date + " יש לסיים את יצירת המשוב. <br/>" +
                                "המערכת השלימה את המשוב באופן מלא<br/> ," +
                                " הינך מוזמן לצפות ולערוך את המשוב בלינק הבא:http://proj.ruppin.ac.il/igroup15/prod/index.html <br/>"+
                                "בתודה , צוות MASHOV";
                SendEMail("arielsayag19@gmail.com", subj, body);
              // SendEMail(manager.Email, subj, body);
            }
           
            return 1;
        }

        return 0;
    }
    //public int ipdate(CritInDoc d)
    //{
    //    int numEffected = 0;
    //    SqlConnection con;
    //    SqlCommand cmd;


    //    try
    //    {
    //        con = connect("con15"); // create the connection
    //    }
    //    catch (Exception ex)
    //    {
    //        // write to log
    //        throw (ex);
    //    }


    //    String cStr = BuildInsertCommandUpdate(d);
    //    cmd = CreateCommand(cStr, con);

    //    try
    //    {
    //        numEffected += cmd.ExecuteNonQuery(); // execute the command


    //    }
    //    catch (Exception ex)
    //    {
    //        // write to log
    //        throw (ex);
    //    }



    //    if (con != null)
    //    {
    //        // close the db connection
    //        con.Close();
    //    }
    //    return numEffected;
    //}
    //private String BuildInsertCommandUpdate(CritInDoc d)
    //{
    //    String command;

    //    StringBuilder sb = new StringBuilder();


    //    String prefix = " UPDATE FeedBack_Doc SET systemupdated = '" + d.Doc.Systemupdated + "', [totalW]='" + d.Doc.TotalWeight + "' WHERE [NumDoc] = '" + d.Doc.NumDoc + "'";
    //    command = prefix + sb.ToString();

    //    return command;
    //}
    //--------------------------------------------------------------------------------------------------
    // This method deletes  CLASSEX
    //--------------------------------------------------------------------------------------------------

    public int delete(int id)
    {
        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }


        StringBuilder sb = new StringBuilder();


        sb.AppendFormat("delete from FeedBack_Criteria where NumDoc ={0}", id);
        String cStr = sb.ToString();
        // helper method to build the insert string

        cmd = CreateCommand(cStr, con);             // create the command

        try
        {
            numEffected += cmd.ExecuteNonQuery(); // execute the command

        }
        catch (Exception ex)
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }


        return numEffected;

    }

    public int delete(CritInDoc f)
    {
        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        StringBuilder sb = new StringBuilder();

        sb.AppendFormat("delete from FeedBack_Criteria where NumDoc ={0} and NumCrit={1}", f.Doc.NumDoc, f.AllCrit[0].NumCrit);
        String cStr = sb.ToString();
        // helper method to build the insert string

        cmd = CreateCommand(cStr, con);             // create the command

        try
        {
            numEffected += cmd.ExecuteNonQuery(); // execute the command

        }
        catch (Exception ex)
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }

        numEffected += updateStatusDoc(f);
        return numEffected;

    }



    /*_________________________________________________________________*/
    /*-------------Insert Users-----------*/
    /*_________________________________________________________________*/

    public int insert(List<Users> users)
    {
        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;


        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        foreach (var item in users)
        {
            item.Pass = GeneratePassword();
            String cStr = BuildInsertCommand(item);
            cmd = CreateCommand(cStr, con);

            try
            {
                numEffected += cmd.ExecuteNonQuery(); // execute the command

                string subject = "סיסמא חדשה למערכת MASHOV , ברוך הבא";
                string body = "<div style='direction: rtl'>שלום , " + item.FirstName;
                body += "<br><br>ברוך הבא למערכת MASHOV, ";
                body += "<br>שם משתמש שלך הינו: " + item.Email + "<br> סיסמתך היא: " + item.Pass;
                body += "<br><br>תודה , צוות MASHOV</div>";
                SendEMail(item.Email, subject, body);

            }

            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                {
                    continue;
                }
                else
                {
                    throw (ex);
                }
            }


        }
        foreach (var item in users)
        {
            String cStr = BuildInsertCommandTypes(item);
            cmd = CreateCommand(cStr, con);

            try
            {
                numEffected += cmd.ExecuteNonQuery(); // execute the command


            }

            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                {
                    continue;
                }
                else
                {
                    throw (ex);
                }
            }


        }

        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        return numEffected;
    }
    //------------------insert Students-------------------//
    public int insert(List<Students> students)
    {
        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;
        SqlCommand cmd1;

        List<int> idGroup = new List<int>();
        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        foreach (var item in students)
        {
            int idG = 0;
            foreach (var tem in item.GroupS)
            {

                String cStr = BuildInsertCommand(item);
                cmd = CreateCommand(cStr, con);

                try
                {
                    numEffected += cmd.ExecuteNonQuery(); // execute the command


                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627)
                    {
                        numEffected += 0;
                    }
                    else
                    {
                        throw (ex);

                    }
                }

                idG = findkeyGroup(tem);
                //idGroup.Add(findkeyGroup(tem));
                String cStr1 = BuildInsertCommand(item, idG);
                cmd1 = CreateCommand(cStr1, con);

                try
                {
                    numEffected += cmd1.ExecuteNonQuery(); // execute the command


                }
                catch (Exception ex)
                {
                    // write to log
                    throw (ex);
                }
            }



        }


        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        return numEffected;
    }
    /*-------------Insert Users-----------*/
    private String BuildInsertCommand(Users user)
    {
        String command;
        StringBuilder sb = new StringBuilder();

        // use a string builder to create the dynamic string
        sb.AppendFormat("Values('{0}', '{1}','{2}','{3}')",
          user.Email, user.FirstName, user.LastName, user.Pass);
        String prefix = "INSERT INTO Users " + "(Email,FirstName,LastName,pass)";
        command = prefix + sb.ToString();

        return command;
    }
    /*-------------Insert UsersTypes-----------*/
    private String BuildInsertCommandTypes(Users user)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string
        sb.AppendFormat("Values('{0}', '{1}')",
         user.Email, user.Type.NumType);
        String prefix = "INSERT INTO UsersType " + "(Email,utID)";
        command = prefix + sb.ToString();

        return command;
    }
    /*-------------Insert Students-----------*/
    private String BuildInsertCommand(Students s)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string
        sb.AppendFormat("Values('{0}', '{1}','{2}','{3}','{4}')",
        s.Id, s.FirstName, s.LastName, s.Email, s.Id);
        String prefix = "INSERT INTO Student " + "(IDStudent,FirstName,LastNAme,Email,pass)";
        command = prefix + sb.ToString();

        return command;
    }
    /*-------------Insert Students-----------*/
    private String BuildInsertCommand(Students s, int keyNumGroup)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string
        sb.AppendFormat("Values('{0}', '{1}')",
        s.Id, keyNumGroup);
        String prefix = "INSERT INTO studentInGroup " + "(IDStudent,NumGroup)";
        command = prefix + sb.ToString();

        return command;
    }

    //-----------------insert Courses AND Department------------//
    public int insert(string course, string dep)
    {
        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;
        SqlCommand cmd1;
        //  SqlCommand cmd2;


        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        if (dep != null)
        {
            String cStr = BuildInsertCommand(dep);
            cmd = CreateCommand(cStr, con);

            try
            {
                numEffected += cmd.ExecuteNonQuery(); // execute the command


            }

            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                {
                    return 0;
                }
                else
                {
                    if (con != null)
                    {
                        con.Close();
                    }
                }
            }
        }
        if (course != null)
        {
            String cStr1 = BuildInsertCommandCourse(course);
            cmd1 = CreateCommand(cStr1, con);

            try
            {
                numEffected += cmd1.ExecuteNonQuery();

            }

            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                {
                    return 0;
                }
                else
                {
                    if (con != null)
                    {
                        con.Close();
                    }
                }
            }
        }


        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        return numEffected;
    }
    //---------------insert to Department----------//
    private String BuildInsertCommand(string Dep)
    {

        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string
        sb.AppendFormat("Values('{0}')", Dep);
        String prefix = "INSERT INTO Department " + "(NameDepartment)";
        command = prefix + sb.ToString();

        return command;
    }
    //---------------insert to Courses----------//
    private String BuildInsertCommandCourse(string course)
    {

        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string
        sb.AppendFormat("Values('{0}')", course);
        String prefix = "INSERT INTO Course " + "(NameCourse)";
        command = prefix + sb.ToString();

        return command;
    }
    //---------------insert to Courses_Department----------//
    private String BuildInsertCommand(CoursesAndDepartment courseDep)
    {

        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string
        sb.AppendFormat("Values('{0}','{1}')", courseDep.NumCourse, courseDep.NumDepartment);
        String prefix = "INSERT INTO Course_Department " + "(NumCourse,NumDepartment)";
        command = prefix + sb.ToString();

        return command;
    }
    private CoursesAndDepartment findKeyDep(CoursesAndDepartment courseDep)
    {
        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = "select NumDepartment from Department where NameDepartment='" + courseDep.NameDepartment + "'";
            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (dr.Read())
            {
                courseDep.NumDepartment = Convert.ToInt32(dr["NumDepartment"]);
            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return courseDep;
    }
    private CoursesAndDepartment findKeyCorse(CoursesAndDepartment courseDep)
    {
        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = "select NumCourse from Course where NameCourse='" + courseDep.NameCourse + "'";
            SqlCommand cmd = new SqlCommand(selectSTR, con);

            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (dr.Read())
            {
                courseDep.NumCourse = Convert.ToInt32(dr["NumCourse"]);
            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return courseDep;
    }
    //---------------foundIDOfGroupe------//
    private int findkeyGroup(Groups g)
    {
        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = "select max([NumGroup]) as maxId from [Groups] where [NameGroup]='" + g.NameGroup + "'";
            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (dr.Read())
            {
                g.NumGroup = Convert.ToInt32(dr["maxId"]);
            }
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return g.NumGroup;

    }

    //----------------insert all groups------------------------//
    public int insert(List<Groups> allGroups)
    {
        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;


        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        foreach (var item in allGroups)
        {

            String cStr = BuildInsertCommand(item);
            cmd = CreateCommand(cStr, con);

            try
            {
                numEffected += cmd.ExecuteNonQuery(); // execute the command


            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                {
                    continue;
                }
                else
                {
                    throw (ex);
                }
            }


        }
        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        return numEffected;
    }
    ////------------------foundIDMEntor-----------/
    //private string funcMentorID(Groups g)
    //{
    //    SqlConnection con = null;
    //    string IDMentor = "";
    //    try
    //    {
    //        con = connect("con15");
    //        String selectSTR = "select u.ID from Users u inner join Groups g where u.FirstName='" + g.NameMentor + "' and u.utID='Mentor'";
    //        SqlCommand cmd = new SqlCommand(selectSTR, con);
    //        SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
    //        while (dr.Read())
    //        {
    //            IDMentor = (string)dr["u.ID"];
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        // write to log
    //        throw (ex);
    //    }
    //    finally
    //    {
    //        if (con != null)
    //        {
    //            con.Close();
    //        }

    //    }
    //    return IDMentor;
    //}


    /*-------------Insert Group-----------*/
    private String BuildInsertCommand(Groups g)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string
        sb.AppendFormat("Values('{0}', '{1}','{2}', '{3}','{4}','{5}','{6}','{7}')",
        g.NameGroup, g.NameProject, g.NameOrganization, g.ProjectType, g.Mentor.Email, g.Mentor.Type.NumType, 0, "");
        String prefix = "INSERT INTO Groups " + "(NameGroup,NameProject,NameOrganization,ProjectType,EmailMentor,utID,finalScore,linkPP)";
        command = prefix + sb.ToString();

        return command;
    }
    //-----------------insertallMetting------------------//
    public int insert(FeedBack_Meeting feed)
    {
        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;
        SqlCommand cmd1;

        //insert(feed.DetailsCourseDep.NameCourse, feed.DetailsCourseDep.NameDepartment);
        //feed.DetailsCourseDep=findKeyDep(feed.DetailsCourseDep);
        // feed.DetailsCourseDep = findKeyCorse(feed.DetailsCourseDep);
        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }


        String cStr = BuildInsertCommand(feed);
        cmd = CreateCommand(cStr, con);

        try
        {
            numEffected += cmd.ExecuteNonQuery(); // execute the command


        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        String cStr1 = BuildInsertCommand(feed.DetailsCourseDep);
        cmd1 = CreateCommand(cStr1, con);
        try
        {

            numEffected += cmd1.ExecuteNonQuery();

        }
        catch (SqlException ex)
        {
            if (ex.Number == 2627)
            {
                numEffected++;
            }
            else
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        return numEffected;
    }
    /*-------------Insert Mettings-----------*/
    private String BuildInsertCommand(FeedBack_Meeting f)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string
        sb.AppendFormat("Values('{0}', '{1}','{2}', '{3}','{4}')",
        f.NameMeeting, f.Date, f.YearMeeting, f.DetailsCourseDep.NumCourse, f.DetailsCourseDep.NumDepartment);
        String prefix = "INSERT INTO FeedBack_Meet " + "(nameMetting,DateMeeting,yearMeeting,NumCourse,NumDepartment)";
        command = prefix + sb.ToString();

        return command;
    }
    //---------------foundIDMeeting------//
    private int foundIDFeedBack_Session(FeedBack_Meeting f)
    {
        SqlConnection con = null;
        int IDFeedBack_Session = 0;
        try
        {
            con = connect("con15");
            String selectSTR = "select max([NumMeeting]) as maxId from FeedBack_Meet";
            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (dr.Read())
            {
                IDFeedBack_Session = Convert.ToInt32(dr["maxId"]);
            }
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return IDFeedBack_Session;

    }
    //-----------------insertallGroupMetting------------------//
    public int insert(List<Group_Meeting> allMettings, FeedBack_Meeting f, FeedBack_Doc doc)
    {
        insert(f);
        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;
        SqlCommand cmd1;

        int IDmeet = foundIDFeedBack_Session(f);
        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        doc.DetailsMeet = new FeedBack_Meeting();
        doc.DetailsMeet.NumMeeting = IDmeet;
        insert(doc);
        foreach (var item in allMettings)
        {
            item.Group.NumGroup = findkeyGroup(item.Group);
            String cStr = BuildInsertCommand(item, IDmeet);
            cmd = CreateCommand(cStr, con);
            try
            {
                numEffected += cmd.ExecuteNonQuery(); // execute the command


            }
            catch (Exception ex)
            {
                throw (ex);
            }

            foreach (var tem in item.JudgesGroup)
            {

                String cStr1 = BuildInsertCommand(item, tem.Judge.Email);
                cmd1 = CreateCommand(cStr1, con);

                try
                {
                    numEffected += cmd1.ExecuteNonQuery();

                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627)
                    {
                        continue;
                    }
                    else
                    {
                        throw (ex);
                    }
                }
            }
        }

        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        return numEffected;
    }

    /*-------------Insert FeedBack_Session_Groups-----------*/
    private String BuildInsertCommand(Group_Meeting gm, int m)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string
        sb.AppendFormat("Values('{0}', '{1}')",
        m, gm.Group.NumGroup);
        String prefix = "INSERT INTO FeedBack_Meet_Groups " + "(NumMeeting,NumGroup)";
        command = prefix + sb.ToString();

        return command;
    }
    //------------------insert-Judge_Groups----------------//
    private String BuildInsertCommand(Group_Meeting gm, string EJudge)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string
        sb.AppendFormat("Values('{0}', '{1}','{2}','{3}','{4}','{5}')",
        EJudge, gm.Group.NumGroup, 3, 0, gm.StartTime, gm.EndTime);
        String prefix = "INSERT INTO Judge_Groups " + "(EmailJudge,NumGroup,utID,sumScore,startTime,endTime)";
        command = prefix + sb.ToString();

        return command;

    }


    //------------------insert-Doc----------------//
    public int insert(FeedBack_Doc feed)
    {
        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;
        SqlCommand cmd1;


        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }


        String cStr = BuildInsertCommand(feed);
        cmd = CreateCommand(cStr, con);

        try
        {
            numEffected += cmd.ExecuteNonQuery(); // execute the command


        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        feed.NumDoc = findIDDOC(feed);
        foreach (var item in feed.Manager)
        {

            String cStr1 = BuildInsertCommand(item, feed.NumDoc);
            cmd1 = CreateCommand(cStr1, con);
            try
            {
                numEffected += cmd1.ExecuteNonQuery(); // execute the command


            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }
        }
        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        return numEffected;
    }

    //------------------insert-Doc----------------//
    private String BuildInsertCommand(FeedBack_Doc d)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string
        sb.AppendFormat("Values('{0}', '{1}','{2}','{3}')",
        d.NameDoc, d.DetailsMeet.NumMeeting, 0, 0);
        String prefix = "INSERT INTO FeedBack_Doc " + "(NameDoc,NumMeeting,systemupdated,totalW)";
        command = prefix + sb.ToString();

        return command;

    }
    //------------------insert-DocManager----------------//
    private String BuildInsertCommand(Users d, int numDoc)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string
        sb.AppendFormat("Values('{0}', '{1}','{2}')",
        numDoc, d.Email, d.Type.NumType);
        String prefix = "INSERT INTO Manager_feedback " + "(NumDoc,EmailManager,utID)";
        command = prefix + sb.ToString();

        return command;

    }
    //---------------foundIDDoc------//
    private int findIDDOC(FeedBack_Doc d)
    {
        SqlConnection con = null;
        int id = 0;
        try
        {
            con = connect("con15");
            String selectSTR = "select max([NumDoc]) as maxId from FeedBack_Doc where [NameDoc]='" + d.NameDoc + "' AND [NumMeeting]='" + d.DetailsMeet.NumMeeting + "'";
            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (dr.Read())
            {
                id = Convert.ToInt32(dr["maxId"]);
            }
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return id;

    }
    //---------------foundIDOfCriteria------//
    private int findkeyCrit(Criterion c)
    {
        SqlConnection con = null;
        int id = 0;
        try
        {
            con = connect("con15");
            String selectSTR = "select max([NumCrit]) as maxId from [Criteria] where [NameCrit]='" + c.NameCrit + "' AND [DescriptionCrit]='" + c.DescriptionCrit + "'";
            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (dr.Read())
            {
                id = Convert.ToInt32(dr["maxId"]);
            }
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return id;

    }

    //----------------------------------------------------------------------------------------------//
    //----------------------------שלב שני - בניית המשוב .הכנסת קריטריונים------------------------//
    //----------------------------------------------------------------------------------------------//


    //-----------------insertCriterion------------------//
    public List<Criterion> insert(List<Criterion> allCrit)
    {

        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;


        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        foreach (var item in allCrit)
        {
            if (item.NumCrit == 0)
            {


                String cStr = BuildInsertCommand(item);
                cmd = CreateCommand(cStr, con);
                try
                {
                    numEffected += cmd.ExecuteNonQuery(); // execute the command


                }
                catch (Exception ex)
                {
                    throw (ex);
                }
                item.NumCrit = findkeyCrit(item);
            }

        }

        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        return allCrit;
    }
    //------------------insert-Criterion----------------//
    private String BuildInsertCommand(Criterion c)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string
        sb.AppendFormat("Values('{0}', '{1}','{2}')",
        c.NameCrit, c.DescriptionCrit, 0);
        String prefix = "INSERT INTO Criteria " + "(NameCrit,DescriptionCrit,counterScore)";
        command = prefix + sb.ToString();

        return command;

    }
    //----------insertCritINDoc-------------------//
    public List<Criterion> insert(CritInDoc fullDoc, bool a = false)
    {
        if (!a) { fullDoc.AllCrit = insert(fullDoc.AllCrit); }

        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;


        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        foreach (var item in fullDoc.AllCrit)
        {

            String cStr = BuildInsertCommand(item, fullDoc.Doc.NumDoc);
            cmd = CreateCommand(cStr, con);
            try
            {
                numEffected += cmd.ExecuteNonQuery(); // execute the command


            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                {
                    continue;
                }
                else
                {
                    throw (ex);
                }
            }

        }


        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        numEffected += updateStatusDoc(fullDoc);
        return fullDoc.AllCrit;
    }
    //----------insertCritINDocLastDoc-------------------//
    public int updateDocwithLastDoc(CritInDoc fullDoc)
    {
        delete(fullDoc.Doc.NumDoc);
        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;


        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        foreach (var item in fullDoc.AllCrit)
        {

            String cStr = BuildInsertCommand(item, fullDoc.Doc.NumDoc);
            cmd = CreateCommand(cStr, con);
            try
            {
                numEffected += cmd.ExecuteNonQuery(); // execute the command


            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }


        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        numEffected += updateStatusDoc(fullDoc);
        return numEffected;
    }
    //----------------------updateStatus--------------//

    public int updateStatusDoc(CritInDoc fullDoc)
    {

        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;



        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }



        String cStr = BuildInsertCommandupdate(fullDoc.Doc);
        cmd = CreateCommand(cStr, con);
        try
        {
            numEffected += cmd.ExecuteNonQuery(); // execute the command


        }
        catch (Exception ex)
        {
            throw (ex);
        }



        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        return numEffected;
    }
    //-------------------insertCritINDoc----------------//
    private String BuildInsertCommand(Criterion c, int numDoc)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string
        sb.AppendFormat("Values('{0}', '{1}','{2}','{3}')",
        c.NumCrit, numDoc, c.WeightCrit, c.TypeCrit);
        String prefix = "INSERT INTO FeedBack_Criteria " + "(NumCrit,NumDoc,WeightCrit,numScala)";
        command = prefix + sb.ToString();

        return command;

    }
    //-------------------insertCritINDoc----------------//
    private String BuildInsertCommandupdate(FeedBack_Doc f)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string

        String prefix = "UPDATE FeedBack_Doc Set [status] = '" + f.Status + "' , totalW='" + f.TotalWeight + "'  where[NumDoc] = '" + f.NumDoc + "'";
        command = prefix + sb.ToString();

        return command;

    }





    //----------------------------------------------------------------------------------------------//
    //--------------------------------------------Login Page ---------------------------------------//
    //----------------------------------------------------------------------------------------------//


    public Users checkingExistUser(Users u)
    {

        Users user = new Users();
        user.Typesofuser = new List<Types>();

        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = @"select u.FirstName , u.LastName ,u.Email ,u.pass ,tu.utID , t.TypeUser as TypeUser  
                                from Users u inner join UsersType tu  on u.Email=tu.Email inner join [Type] t on tu.utID=t.utID
                                where u.Email='" + u.Email + "'";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {

                Types t = new Types();
                user.Email = (string)dr["Email"];
                user.Pass = (string)dr["pass"];
                user.FirstName = (string)dr["FirstName"];
                user.LastName = (string)dr["LastName"];
                t.Type = (string)dr["TypeUser"];
                t.NumType = Convert.ToInt32(dr["utID"]);
                user.Typesofuser.Add(t);


            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return user;
    }

    public Students checkingExist(Students u)
    {



        Students s = new Students();
        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = " select * from Student where IDStudent='" + u.Id + "'";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                s.Id = (string)dr["IDStudent"];
                s.Pass = (string)dr["pass"];
                s.FirstName = (string)dr["FirstName"];
                s.LastName = (string)dr["LastName"];
                s.Email = (string)dr["Email"];

            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return s;
    }
    //---------------------------------------update-Pass-User-------------------------------//
    public int updetpass(Users u)
    {

        SqlConnection con;
        SqlCommand cmd;
        int numEffected = 0;
        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        String cStr = BuildInsertCommandUpdate(u); // helper method to build the insert string
        cmd = CreateCommand(cStr, con);
        try
        {
            numEffected = cmd.ExecuteNonQuery(); // execute the command

        }
        catch (Exception ex)
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
            // write to log
            throw (ex);
        }

        // create the command 
        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        return numEffected;
    }
    private String BuildInsertCommandUpdate(Users u)
    {
        String command;

        StringBuilder sb = new StringBuilder();


        String prefix = " UPDATE Users SET pass = '" + u.Pass + "' WHERE Email = '" + u.Email + "'";
        command = prefix + sb.ToString();

        return command;
    }
    public int updetpass(Students s)
    {

        SqlConnection con;
        SqlCommand cmd;
        int numEffected = 0;
        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        String cStr = BuildInsertCommandUpdate(s); // helper method to build the insert string
        cmd = CreateCommand(cStr, con);
        try
        {
            numEffected = cmd.ExecuteNonQuery(); // execute the command

        }
        catch (Exception ex)
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
            // write to log
            throw (ex);
        }

        // create the command 
        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        return numEffected;
    }
    private String BuildInsertCommandUpdate(Students s)
    {
        String command;

        StringBuilder sb = new StringBuilder();


        String prefix = " UPDATE [Student] SET pass = '" + s.Pass + "' WHERE IDStudent = '" + s.Id + "'";
        command = prefix + sb.ToString();

        return command;
    }

    //-------------------updatemailStudent-----------------------------------------------------//
    public Students updateEmailStudent(Students s)
    {

        SqlConnection con;
        SqlCommand cmd;
        int numEffected = 0;
        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        String cStr = BuildInsertCommandUpdateEmail(s); // helper method to build the insert string
        cmd = CreateCommand(cStr, con);
        try
        {
            numEffected = cmd.ExecuteNonQuery(); // execute the command

        }
        catch (Exception ex)
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
            // write to log
            throw (ex);
        }

        // create the command 
        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        return checkingExist(s);
        ;
    }
    private String BuildInsertCommandUpdateEmail(Students s)
    {
        String command;

        StringBuilder sb = new StringBuilder();


        String prefix = " UPDATE [Student] SET Email = '" + s.Email + "' WHERE IDStudent = '" + s.Id + "'";
        command = prefix + sb.ToString();

        return command;
    }


    //----------------------------------------------------------------------------------------------//
    //--------------------------------------------Get User Deatails---------------------------------//
    //----------------------------------------------------------------------------------------------//

    //----------------------------------------manager---------------------------------------//
    public List<FeedBack_Doc> get(Users u)
    {


        List<FeedBack_Doc> doc = new List<FeedBack_Doc>();
        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = @"select * from FeedBack_Doc d inner join FeedBack_Meet m on 
                               d.NumMeeting=m.NumMeeting inner join Manager_feedback a on a.NumDoc = d.NumDoc inner join Course c 
                               on c.NumCourse = m.NumCourse inner join Department de on de.NumDepartment = m.NumDepartment 
                               where a.EmailManager='" + u.Email + "' and a.utID='" + u.Type.NumType + "'";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                FeedBack_Doc d = new FeedBack_Doc();
                d.DetailsMeet = new FeedBack_Meeting();
                d.DetailsMeet.DetailsCourseDep = new CoursesAndDepartment();
                d.Manager = new List<Users>();

                d.NumDoc = Convert.ToInt32(dr["NumDoc"]);
                d.NameDoc = (string)dr["NameDoc"];
                d.DetailsMeet.NumMeeting = Convert.ToInt32(dr["NumMeeting"]);
                d.DetailsMeet.NameMeeting = (string)dr["nameMetting"];
                d.DetailsMeet.YearMeeting = (string)dr["yearMeeting"];
                d.DetailsMeet.DetailsCourseDep.NumCourse = Convert.ToInt32(dr["NumCourse"]);
                d.DetailsMeet.DetailsCourseDep.NumDepartment = Convert.ToInt32(dr["NumDepartment"]);
                d.DetailsMeet.DetailsCourseDep.NameDepartment = (string)dr["NameDepartment"];
                d.DetailsMeet.DetailsCourseDep.NameCourse = (string)dr["NameCourse"];
                d.DetailsMeet.Date = (string)dr["DateMeeting"];
                d.Status = (bool)dr["status"];
              
                d.TotalWeight = Convert.ToDouble(dr["totalW"]);
                d.Manager.Add(u);

                doc.Add(d);

            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return doc;
    }

    //-----------------------------------------get manager Doc+Crit[]----------------------------------------------//
    public CritInDoc get(FeedBack_Doc d)
    {


        CritInDoc critList = new CritInDoc();
        critList.AllCrit = new List<Criterion>();
        SqlConnection con = null;

        try
        {
            con = connect("con15");

            String selectSTR = @"select cr.NumCrit,cr.NameCrit,cr.DescriptionCrit ,c.WeightCrit,c.numScala ,s.nameScala 
                                from FeedBack_Doc d inner join[dbo].[FeedBack_Criteria] c on d.NumDoc=c.NumDoc 
                                inner join[dbo].[Criteria] cr on cr.NumCrit=c.NumCrit inner join Scala s 
                                on c.numScala=s.numScala where d.NumDoc='" + d.NumDoc + "'";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                Criterion c = new Criterion();
                c.NumCrit = Convert.ToInt32(dr["NumCrit"]);
                c.NameCrit = (string)dr["NameCrit"];
                c.WeightCrit = Convert.ToDouble(dr["WeightCrit"]);
                c.TypeCrit = Convert.ToInt32(dr["numScala"]);
                c.NameScala = (string)dr["nameScala"];
                c.DescriptionCrit = (string)dr["DescriptionCrit"];



                critList.AllCrit.Add(c);

            }
            critList.Doc = d;

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return critList;
    }

    //----------------------------------Get-Doc--------------------------------------//

    public FeedBack_Doc getDoc()
    {

        FeedBack_Doc d = new FeedBack_Doc();
        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = @"select NumDoc, NameDoc, NumMeeting, [status] from FeedBack_Doc 
                                 where NumDoc = (select max([NumDoc]) from FeedBack_Doc)";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {

                d.DetailsMeet = new FeedBack_Meeting();
                d.DetailsMeet.DetailsCourseDep = new CoursesAndDepartment();
                d.Manager = new List<Users>();

                d.NumDoc = Convert.ToInt32(dr["NumDoc"]);
                d.NameDoc = (string)dr["NameDoc"];
                d.DetailsMeet.NumMeeting = Convert.ToInt32(dr["NumMeeting"]);
                d.Status = (bool)dr["status"];




            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return d;
    }

    //------------------------------get all dep list-----------------------------------------//
    public List<Department> getAllDep()
    {
        List<Department> allDep = new List<Department>();

        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = "select * from Department";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                Department d = new Department();
                d.NumDepartment = Convert.ToInt32(dr["NumDepartment"]);
                d.NameDepartment = (string)dr["NameDepartment"];

                allDep.Add(d);

            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return allDep;
    }
    //-------------------------------get all heb year-----------------------------------//

    public List<string> getAllHebYear()
    {
        List<string> allYears = new List<string>();

        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = "select distinct yearMeeting from FeedBack_Meet";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                string y = "";

                y = (string)dr["yearMeeting"];

                allYears.Add(y);

            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return allYears;
    }
    //-------------------------------get all Courses-----------------------------------//

    public List<Courses> getAllCourses()
    {
        List<Courses> allCourses = new List<Courses>();

        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = "select * from Course";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                Courses c = new Courses();
                c.NumCourse = Convert.ToInt32(dr["NumCourse"]);
                c.NameCourse = (string)dr["NameCourse"];

                allCourses.Add(c);

            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return allCourses;
    }
    public List<Scala> getListScala()
    {
        List<Scala> allScala = new List<Scala>();

        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = "select * from Scala";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                Scala s = new Scala();
                s.NumScala = Convert.ToInt32(dr["numScala"]);
                s.NameScala = (string)dr["nameScala"];

                allScala.Add(s);

            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return allScala;
    }
    //-----------------------Setting---------------------------------------------------------//
    //-----------------------GET-Group---------------------------------------------------------//
    public List<Group_Meeting> getGroup(int numMeet)
    {

        List<Group_Meeting> listGroups = new List<Group_Meeting>();


        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = @"select g.NumGroup, g.NameGroup , g.NameProject,jg.startTime , jg.endTime 
                               from Groups g inner join[dbo].[FeedBack_Meet_Groups] n on g.NumGroup=n.NumGroup
                               inner join [dbo].[FeedBack_Meet] m on n.NumMeeting=m.NumMeeting 
                               inner join[dbo].[FeedBack_Doc] d on d.NumMeeting=m.NumMeeting 
                               inner join Judge_Groups jg on jg.NumGroup=g.NumGroup 
                               where d.NumDoc='" + numMeet + "'";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


            while (dr.Read())
            {
                Group_Meeting gm = new Group_Meeting();
                gm.Group = new Groups();
                //  Groups g = new Groups();
                gm.Group.NumGroup = Convert.ToInt32(dr["NumGroup"]);
                gm.Group.NameGroup = (string)dr["NameGroup"];
                gm.Group.NameProject = (string)dr["NameProject"];
                gm.StartTime = (string)dr["startTime"];
                gm.EndTime = (string)dr["endTime"];

                listGroups.Add(gm);

            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }

        return listGroups;



    }

    //-----------------------GET-USER---------------------------------------------------------//
    public List<Users> getUsers()
    {

        List<Users> listUser = new List<Users>();

        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = "select * from [Users]";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


            while (dr.Read())
            {
                Users u = new Users();
                u.FirstName = (string)dr["FirstName"];
                u.LastName = (string)dr["LastName"];
                u.Email = (string)dr["Email"];


                listUser.Add(u);

            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return listUser;


    }
    //-----------------------GET-Student---------------------------------------------------------//
    public List<Students> getStudent(int numMeet)
    {

        List<Students> listStudents = new List<Students>();

        SqlConnection con = null;

        try
        {
            con = connect("con15");

            String selectSTR = "select s.FirstName , s.LastNAme ,s.IDStudent ,g.NumGroup , g.NameProject from Student s inner join studentInGroup sg on s.IDStudent=sg.IDStudent inner join Groups g on g.NumGroup=sg.NumGroup inner join FeedBack_Meet_Groups fg on fg.NumGroup=g.NumGroup inner join FeedBack_Doc d on d.NumMeeting=fg.NumMeeting  where d.NumDoc='" + numMeet + "'";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


            while (dr.Read())
            {
                Students st = new Students();
                st.GroupS = new List<Groups>();
                Groups g = new Groups();
                st.FirstName = (string)dr["FirstName"];
                st.Id = (string)dr["IDStudent"];
                st.LastName = (string)dr["LastNAme"];
                g.NumGroup = Convert.ToInt32(dr["NumGroup"]);
                g.NameProject = (string)dr["NameProject"];


                st.GroupS.Add(g);

                listStudents.Add(st);

            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return listStudents;


    }
    //-----------------------GET-Type--------------------------------------------------------//
    public List<Types> getType()
    {

        List<Types> listTypes = new List<Types>();

        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = "select * from [Type]";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


            while (dr.Read())
            {
                Types t = new Types();
                t.NumType = Convert.ToInt32(dr["utID"]);
                t.Type = (string)dr["TypeUSer"];

                listTypes.Add(t);

            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return listTypes;


    }
    //-----------------insertNewJudgeToGroup-Setting------------------//
    public int insert(Group_Meeting group_Meet)
    {

        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd1;


        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }


        String cStr1 = BuildInsertCommand(group_Meet, group_Meet.JudgesGroup[0].Judge.Email);
        cmd1 = CreateCommand(cStr1, con);

        try
        {
            numEffected += cmd1.ExecuteNonQuery();

        }
        catch (SqlException ex)
        {
            if (ex.Number == 2627)
            {
                numEffected += 0;
            }
            else
            {
                throw (ex);
            }
        }

        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        return numEffected;
    }
    //-------------------------update  student in group---------------------//
    public int updateStudentInGroup(Students s)
    {
        List<int> oldNumG = checkednameProject(s);
        List<int> newNumG = neNumGroups(s);


        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd1;


        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        int j = 0;
        foreach (var item in oldNumG)
        {

            String cStr1 = BuildInsertCommandupdate(item, newNumG[j], s.Id);//updet student in group 
            cmd1 = CreateCommand(cStr1, con);


            try
            {
                numEffected += cmd1.ExecuteNonQuery();
                j++;

            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                {
                    numEffected += 0;
                }
                else
                {
                    throw (ex);
                }
            }

        }
        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        return numEffected;
    }
    /*-------------update studentInGroup-----------*/
    private String BuildInsertCommandupdate(int oldNumGroup, int newNymGroup, string idOfSelectedStudent)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        String prefix = "update studentInGroup set NumGroup='" + newNymGroup + "'where IDStudent='" + idOfSelectedStudent + "'and NumGroup='" + oldNumGroup + "'";
        command = prefix + sb.ToString();

        return command;
    }
    //------------get all old Id groups for student -------------------------//
    private List<int> checkednameProject(Students s)
    {
        List<int> listOldIDG = new List<int>();

        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = "select g.NumGroup  from Groups g inner join studentInGroup sg on g.NumGroup=sg.NumGroup inner join Student s on s.IDStudent=sg.IDStudent where s.IDStudent='" + s.Id + "' and g.NameProject='" + s.GroupS[0].NameProject + "'";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


            while (dr.Read())
            {
                int old = Convert.ToInt32(dr["NumGroup"]);

                listOldIDG.Add(old);

            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return listOldIDG;

    }
    //--------------------------get all new id groups fro student---------------------------------//
    private List<int> neNumGroups(Students s)
    {
        List<int> listNewIDG = new List<int>();

        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = " select distinct  g.NumGroup from Groups g inner join studentInGroup sg on g.NumGroup = sg.NumGroupwhere g.NameOrganization = '" + s.GroupS[1].NameProject + "'";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


            while (dr.Read())
            {
                int newid = Convert.ToInt32(dr["NumGroup"]);


                listNewIDG.Add(newid);

            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return listNewIDG;
    }
    //-----------------------------------END Setting--------------------------------//
    //-------------------------------------------------------------------------------------//
    //-----------------------------------update Crit-------------------------//
    public List<Criterion> insAndUpCrit(CritInDoc c)
    {
        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;
        int newID;
        int oldId;

        oldId = c.AllCrit[0].NumCrit;
        bool isExisting = existingCrit(c.AllCrit[0]);

        if (!isExisting)//if not exist 
        {
            try
            {
                con = connect("con15"); // create the connection
            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }



            String cStr = BuildInsertCommand(c.AllCrit[0]);
            cmd = CreateCommand(cStr, con);

            try
            {
                numEffected += cmd.ExecuteNonQuery(); // execute the command


            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }
            if (con != null)
            {
                // close the db connection
                con.Close();
            }

        }

        newID = findkeyCrit(c.AllCrit[0]);
        c.AllCrit[0].NumCrit = newID;
        numEffected += updateIDcritAndDoc(c, oldId);
        return c.AllCrit;
    }
    public bool existingCrit(Criterion c1)
    {
        bool yesOrNo = false;
        SqlConnection con = null;
        try
        {
            con = connect("con15");
            String selectSTR = "select * from Criteria where NameCrit='" + c1.NameCrit + "' and DescriptionCrit='" + c1.DescriptionCrit + "'";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


            while (dr.Read())
            {
                Criterion c = new Criterion();
                c.NumCrit = Convert.ToInt32(dr["NumCrit"]);
                c.NameCrit = (string)dr["NameCrit"];
                c.DescriptionCrit = (string)dr["DescriptionCrit"];


                yesOrNo = true;

            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }

        return yesOrNo;
    }
    public int updateIDcritAndDoc(CritInDoc cd, int oldId)
    {
        SqlConnection con;
        SqlCommand cmd;
        int numEffected = 0;
        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        String cStr = BuildInsertCommandUpdate(cd, oldId); // helper method to build the insert string
        cmd = CreateCommand(cStr, con);
        try
        {
            numEffected = cmd.ExecuteNonQuery(); // execute the command

        }
        catch (Exception ex)
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
            // write to log
            throw (ex);
        }

        // create the command 
        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        return numEffected;

    }
    private String BuildInsertCommandUpdate(CritInDoc cd, int oldId)
    {
        String command;

        StringBuilder sb = new StringBuilder();
        String prefix = "UPDATE FeedBack_Criteria Set [NumCrit] = '" + cd.AllCrit[0].NumCrit + "' , [WeightCrit]='" + cd.AllCrit[0].WeightCrit + "' , [numScala]='" + cd.AllCrit[0].TypeCrit + "' where [NumDoc]='" + cd.Doc.NumDoc + "' AND [NumCrit]='" + oldId + "'";
        command = prefix + sb.ToString();

        return command;

    }
    //-----------------------------Bank Criteria--------------------------------------//
    public List<Criterion> getAllCrit()
    {

        List<Criterion> listC = new List<Criterion>();

        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = "SELECT a.* FROM Criteria a INNER JOIN (SELECT NameCrit,DescriptionCrit, MIN(NumCrit) as NumCrit FROM Criteria GROUP BY NameCrit,DescriptionCrit ) AS b ON a.NameCrit = b.NameCrit  AND a.NumCrit = b.NumCrit";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


            while (dr.Read())
            {
                Criterion c = new Criterion();
                c.NumCrit = Convert.ToInt32(dr["NumCrit"]);
                c.NameCrit = (string)dr["NameCrit"];
                c.DescriptionCrit = (string)dr["DescriptionCrit"];

                listC.Add(c);

            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return listC;

    }
    //----------------------------------------Judge---------------------------------------//
    public List<FeedBack_Meeting> getJudgeCourse(Users u)
    {


        List<FeedBack_Meeting> doc = new List<FeedBack_Meeting>();
        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = @"select distinct m.NumMeeting, m.DateMeeting,m.yearMeeting,m.nameMetting,d.NameDepartment,
                               c.NameCourse,c.NumCourse,d.NumDepartment,f1.NumDoc from Judge_Groups jg inner join FeedBack_Meet_Groups mg 
                               on jg.NumGroup=mg.NumGroup inner join FeedBack_Meet m on m.NumMeeting=mg.NumMeeting inner join
                               Course c on c.NumCourse=m.NumCourse inner join Department d on d.NumDepartment=m.NumDepartment 
                               inner join FeedBack_Doc f1 on f1.NumMeeting=m.NumMeeting
                               where jg.EmailJudge='" + u.Email + "' and jg.utID='" + u.Type.NumType + "'";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {

                FeedBack_Meeting DetailsMeet = new FeedBack_Meeting();
                DetailsMeet.DetailsCourseDep = new CoursesAndDepartment();

                DetailsMeet.NumMeeting = Convert.ToInt32(dr["NumMeeting"]);
                DetailsMeet.NumDoc = Convert.ToInt32(dr["NumDoc"]);
                DetailsMeet.NameMeeting = (string)dr["nameMetting"];
                DetailsMeet.YearMeeting = (string)dr["yearMeeting"];
                DetailsMeet.DetailsCourseDep.NumCourse = Convert.ToInt32(dr["NumCourse"]);
                DetailsMeet.DetailsCourseDep.NumDepartment = Convert.ToInt32(dr["NumDepartment"]);
                DetailsMeet.DetailsCourseDep.NameDepartment = (string)dr["NameDepartment"];
                DetailsMeet.DetailsCourseDep.NameCourse = (string)dr["NameCourse"];
                DetailsMeet.Date = (string)dr["DateMeeting"];


                doc.Add(DetailsMeet);

            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return doc;
    }
    //----------------------------------------GetAllGroupsByMetting---------------------------------------//
    public List<Group_Meeting> getJudgeGroups(Users u, int numMeet)
    {


        List<Group_Meeting> allGroup = new List<Group_Meeting>();
        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = @"select distinct g.NumGroup,g.NameGroup,g.NameProject,g.NameOrganization,g.linkPP ,u.FirstName,
                               u.LastName,jg.startTime,jg.endTime ,jg.sumScore from Groups g inner join Judge_Groups jg on 
                               g.NumGroup=jg.NumGroup inner join  FeedBack_Meet_Groups mg on mg.NumGroup = jg.NumGroup inner 
                               join FeedBack_Meet m on m.NumMeeting = mg.NumMeeting inner join Users u on u.Email = g.EmailMentor
                               where jg.EmailJudge = '" + u.Email + "' and jg.utID='" + u.Type.NumType + "' and mg.NumMeeting = '" + numMeet + "'";


            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                Group_Meeting gm = new Group_Meeting();
                gm.Group = new Groups();
                gm.Group.Mentor = new Users();
                //  FeedBack_Meeting DetailsMeet = new FeedBack_Meeting();
                //DetailsMeet.DetailsCourseDep = new CoursesAndDepartment();

                gm.Group.NumGroup = Convert.ToInt32(dr["NumGroup"]);
                gm.Group.NameGroup = (string)dr["NameGroup"];
                gm.Group.NameProject = (string)dr["NameProject"];
                gm.Group.NameOrganization = (string)dr["NameOrganization"];
                gm.Group.Mentor.FirstName = (string)dr["FirstName"];
                gm.Group.Mentor.LastName = (string)dr["LastName"];
                gm.StartTime = (string)dr["startTime"];
                gm.EndTime = (string)dr["endTime"];
                gm.Sum = Convert.ToDouble(dr["sumScore"]);
                gm.Group.Link = (string)dr["linkPP"];



                allGroup.Add(gm);

            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        foreach (var item in allGroup)
        {
            item.Group.ListStudent= new List<Students>();
            item.Group.ListStudent = foundStudents(item.Group);
            item.StartTime = findTimeStart(item.Group.NumGroup);
            item.EndTime = findTimeEnd(item.Group.NumGroup);
        }

        return allGroup;
    }
    //---------------------------getAllStudentforGroup--------------------------//
    public List<Students> foundStudents(Groups all)
    {
        // List<Group_Meeting> allGroup = new List<Group_Meeting>();
       
            SqlConnection con = null;
            all.ListStudent = new List<Students>();

            try
            {
                con = connect("con15");
                String selectSTR = @"select s.FirstName,s.LastNAme,s.IDStudent from Groups g inner join studentInGroup sg on 
                                   g.NumGroup = sg.NumGroup  inner join Student s on s.IDStudent = sg.IDStudent inner join  
                                   FeedBack_Meet_Groups mg on mg.NumGroup = g.NumGroup where g.NumGroup = '" + all.NumGroup + "'";

                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dr.Read())
                {
                    Students s = new Students();

                    s.FirstName = (string)dr["FirstName"];
                    s.LastName = (string)dr["LastNAme"];
                    s.Id = (string)dr["IDStudent"];

                   all.ListStudent.Add(s);




                }

            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }

            }
        

        return all.ListStudent;
    }
    ////-----------------------------if not edit in the first time-------------------//
    public CritInDoc getMeetDocupdate(int numGroup, Users j)
    {
        CritInDoc testupdate = new CritInDoc();
        testupdate.Doc = new FeedBack_Doc();
        testupdate.AllCrit = new List<Criterion>();
        SqlConnection con = null;
        FullFeedback f = new FullFeedback();
        try
        {
            con = connect("con15");
            String selectSTR = @"select c.NameCrit ,c.DescriptionCrit , c.NumCrit ,f.NumDoc ,f.Score ,f.Note , fc.WeightCrit , 
                                 s.numScala,s.nameScala ,f.valueCrit from Criteria c inner join FeedBack_Criteria fc on 
                                 c.NumCrit = fc.NumCrit inner join Full_feedback f on f.NumCrit = c.NumCrit inner join 
                                 Judge_Groups jg on jg.NumGroup = f.NumGroup inner join Scala s on s.numScala = fc.numScala 
                                 where jg.NumGroup = '" + numGroup + "' and jg.EmailJudge = '" + j.Email + "' and fc.NumDoc='"+j.NumDoc+"'";


            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                Criterion c = new Criterion();

                c.NumCrit = Convert.ToInt32(dr["NumCrit"]);
                c.NameCrit = (string)dr["NameCrit"];
                c.DescriptionCrit = (string)dr["DescriptionCrit"];
                c.WeightCrit = Convert.ToDouble(dr["WeightCrit"]);
                c.TypeCrit = Convert.ToInt32(dr["numScala"]);
                c.NameScala = (string)dr["nameScala"];
                c.Note = (string)dr["Note"];
                c.Score = Convert.ToDouble(dr["Score"]);
                c.ValueCrit = (string)dr["valueCrit"];
                testupdate.Doc.NumDoc = Convert.ToInt32(dr["NumDoc"]);
                testupdate.AllCrit.Add(c);
            }


        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return testupdate;
    }
    //-----------------------------get all critfromDocMeet-------------------//
    public CritInDoc getMeetDoc(int indexMeet)
    {
        CritInDoc test = new CritInDoc();
        test.Doc = new FeedBack_Doc();
        test.AllCrit = new List<Criterion>();
        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = @"select d.NumDoc ,d.NumMeeting,c.NumCrit,c.numScala,s.nameScala, c.WeightCrit,ca.NameCrit,
                               ca.DescriptionCrit from [FeedBack_Doc] d inner join FeedBack_Criteria c on d.NumDoc=c.NumDoc 
                               inner join Criteria ca on ca.NumCrit=c.NumCrit inner join Scala s on s.numScala=c.numScala 
                               where d.NumMeeting='" + indexMeet + "'";


            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                Criterion c = new Criterion();

                c.NumCrit = Convert.ToInt32(dr["NumCrit"]);
                c.NameCrit = (string)dr["NameCrit"];
                c.DescriptionCrit = (string)dr["DescriptionCrit"];
                c.WeightCrit = Convert.ToDouble(dr["WeightCrit"]);
                c.TypeCrit = Convert.ToInt32(dr["numScala"]);
                c.NameScala = (string)dr["nameScala"];
                test.Doc.NumDoc = Convert.ToInt32(dr["NumDoc"]);
                test.AllCrit.Add(c);

            }


        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return test;
    }
    //----------insertAllCritwithAllScoreOfJudge-------------------//
    public int insert(FullFeedback full)
    {

        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;
        SqlCommand cmd1;



        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        foreach (var item in full.CritDoc.AllCrit)
        {

            String cStr = BuildInsertCommand(full, item, full.Judes);
            cmd = CreateCommand(cStr, con);


            try
            {
                numEffected += cmd.ExecuteNonQuery(); // execute the command


            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }

        String cStr1 = BuildInsertCommandupdate(full);
        cmd1 = CreateCommand(cStr1, con);


        try
        {
            numEffected += cmd1.ExecuteNonQuery(); // execute the command


        }
        catch (Exception ex)
        {
            throw (ex);
        }

        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        updateFinalGrade(full.GroupM.Group.NumGroup);
        return numEffected;
    }
    //-------------------insertAllCritwithAllScoreOfJudge----------------//
    private String BuildInsertCommand(FullFeedback full, Criterion c, Users j)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string
        sb.AppendFormat("Values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}','{7}')",
       c.NumCrit, full.CritDoc.Doc.NumDoc, j.Email, j.Type.NumType, full.GroupM.Group.NumGroup, c.Score, c.Note, c.ValueCrit);
        String prefix = "INSERT INTO Full_feedback " + "(NumCrit,NumDoc,EmailJudge,utID,NumGroup,Score,Note,valueCrit)";
        command = prefix + sb.ToString();

        return command;

    }
    //-------------------updateScoreForGroup----------------------------------//
    private String BuildInsertCommandupdate(FullFeedback f)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string

        String prefix = "UPDATE [Judge_Groups] Set [sumScore] = '" + f.Sum + "' , [statusFeed]='" + f.StatusFull + "'   where [NumGroup] = '" + f.GroupM.Group.NumGroup + "' and EmailJudge='" + f.Judes.Email + "'";
        command = prefix + sb.ToString();

        return command;

    }
    //------------------------updateGrage----------------------------//
    private int updateFinalGrade(int index)
    {
        List<int> sumAllJudge = allScoreGroup(index);
        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        float sum = 0;
        foreach (var item in sumAllJudge)
        {
            sum += item;
        }
        sum /= sumAllJudge.Count;
        String cStr = BuildInsertCommandupdateFinal(sum, index);
        cmd = CreateCommand(cStr, con);


        try
        {
            numEffected += cmd.ExecuteNonQuery(); // execute the command


        }
        catch (Exception ex)
        {
            throw (ex);
        }

        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        return numEffected;
    }
    //-------------------updateScoreForGroup----------------------------------//
    private String BuildInsertCommandupdateFinal(double sum, int index)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string

        String prefix = "UPDATE [Groups] Set [finalScore] = '" + sum + "' where [NumGroup] = '" + index + "'";
        command = prefix + sb.ToString();

        return command;

    }
    //------------------------------------------//
    private List<int> allScoreGroup(int idGroup)
    {


        List<int> sum = new List<int>();
        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = @"select sumScore from Judge_Groups where NumGroup='" + idGroup + "'";


            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {

                int sumScoure = Convert.ToInt32(dr["sumScore"]);
                sum.Add(sumScoure);
            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return sum;
    }
    //----------updateAllCritwithAllScoreOfJudge-------------------//
    public int update(FullFeedback full)
    {

        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;
        SqlCommand cmd1;



        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        foreach (var item in full.CritDoc.AllCrit)
        {

            String cStr = BuildInsertCommandupdateFull(full, item, full.Judes);
            cmd = CreateCommand(cStr, con);


            try
            {
                numEffected += cmd.ExecuteNonQuery(); // execute the command


            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }

        String cStr1 = BuildInsertCommandupdate(full);
        cmd1 = CreateCommand(cStr1, con);


        try
        {
            numEffected += cmd1.ExecuteNonQuery(); // execute the command


        }
        catch (Exception ex)
        {
            throw (ex);
        }

        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        updateFinalGrade(full.GroupM.Group.NumGroup);
        return numEffected;
    }
    //-------------------updateScoreForGroup----------------------------------//
    private String BuildInsertCommandupdateFull(FullFeedback f, Criterion c, Users j)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string

        String prefix = "UPDATE [Full_feedback] Set [Score] = '" + c.Score + "', [Note]='" + c.Note + "' , ValueCrit='" + c.ValueCrit + "'  where [NumGroup] = '" + f.GroupM.Group.NumGroup + "' and EmailJudge='" + f.Judes.Email + "' and  NumDoc='" + f.CritDoc.Doc.NumDoc + "' and NumCrit='" + c.NumCrit + "'";
        command = prefix + sb.ToString();

        return command;

    }

    //----------------------------------------Mentor---------------------------------------//
    public List<FeedBack_Meeting> getMentorCourse(Users u)
    {


        List<FeedBack_Meeting> doc = new List<FeedBack_Meeting>();
        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = @"select distinct m.DateMeeting,m.nameMetting,m.NumMeeting,m.yearMeeting,c.NameCourse,c.NumCourse,
                               d.NumDepartment,d.NameDepartment,f1.NumDoc from FeedBack_Meet m inner join FeedBack_Meet_Groups f on 
                               m.NumMeeting = f.NumMeeting inner join Groups g on g.NumGroup = f.NumGroup inner join Course c 
                               on c.NumCourse = m.NumCourse inner join Department d on d.NumDepartment = m.NumDepartment 
                                inner join FeedBack_Doc f1 on f1.NumMeeting=m.NumMeeting
                               where g.EmailMentor = '" + u.Email + "' and g.utID = '" + u.Type.NumType + "'";


            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {

                FeedBack_Meeting DetailsMeet = new FeedBack_Meeting();
                DetailsMeet.DetailsCourseDep = new CoursesAndDepartment();

                DetailsMeet.NumMeeting = Convert.ToInt32(dr["NumMeeting"]);
                DetailsMeet.NumDoc = Convert.ToInt32(dr["NumDoc"]);
                DetailsMeet.NameMeeting = (string)dr["nameMetting"];
                DetailsMeet.YearMeeting = (string)dr["yearMeeting"];
                DetailsMeet.DetailsCourseDep.NumCourse = Convert.ToInt32(dr["NumCourse"]);
                DetailsMeet.DetailsCourseDep.NumDepartment = Convert.ToInt32(dr["NumDepartment"]);
                DetailsMeet.DetailsCourseDep.NameDepartment = (string)dr["NameDepartment"];
                DetailsMeet.DetailsCourseDep.NameCourse = (string)dr["NameCourse"];
                DetailsMeet.Date = (string)dr["DateMeeting"];


                doc.Add(DetailsMeet);

            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return doc;
    }
    //----------------------------------------GetAllGroupsByMetting---------------------------------------//
    public List<Group_Meeting> getMentorGroups(Users u, int numMeet)
    {


        List<Group_Meeting> allGroup = new List<Group_Meeting>();
        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = @"select g.* , m.NumMeeting from FeedBack_Meet m inner join FeedBack_Meet_Groups f on 
                                m.NumMeeting = f.NumMeeting inner join Groups g on g.NumGroup = f.NumGroup                           
                                where g.EmailMentor = '" + u.Email + "' and g.utID = '" + u.Type.NumType + "' and m.NumMeeting = '" + numMeet + "'";



            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                Group_Meeting gm = new Group_Meeting();
                gm.Group = new Groups();
                //gm.Group.Mentor = new Users();

                gm.Group.NumGroup = Convert.ToInt32(dr["NumGroup"]);
                gm.Group.NameGroup = (string)dr["NameGroup"];
                gm.Group.NameProject = (string)dr["NameProject"];
                gm.Group.NameOrganization = (string)dr["NameOrganization"];
                gm.Group.ProjectType = (string)dr["ProjectType"];
                gm.Group.Link = (string)dr["linkPP"];
                gm.Sum = Convert.ToDouble(dr["finalScore"]);
                //gm.EndTime= (string)dr["endTime"];
                //gm.StartTime= (string)dr["startTime"];



                allGroup.Add(gm);

            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }

        foreach (var item in allGroup)
        {
            
            item.JudgesGroup = new List<Judge_Group_Meeting>();
            item.JudgesGroup = findJudgeList(item.Group.NumGroup, numMeet); 

            item.Group.ListStudent = new List<Students>();
            item.Group.ListStudent = foundStudents(item.Group);

            item.StartTime = findTimeStart(item.Group.NumGroup);
            item.EndTime = findTimeEnd(item.Group.NumGroup);
                    
        }
        return allGroup;
    }
    public string findTimeStart(int numGroup)
    {
        string start = "";

        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = @"select distinct jg.startTime from Judge_Groups jg inner join Groups g
	                             on g.NumGroup=jg.NumGroup
	                             where g.NumGroup=" + numGroup + " and jg.startTime<>''";



            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {

                start = (string)dr["startTime"];


            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return start;
    }
    public string findTimeEnd(int numGroup)
    {

        string end = "";
        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = @"select distinct jg.endTime  from Judge_Groups jg inner join Groups g
	                             on g.NumGroup=jg.NumGroup
	                             where g.NumGroup=" + numGroup + " and jg.endTime<>''";



            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {


                end = (string)dr["endTime"];
           


            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return end;
    }
    public List<Judge_Group_Meeting> findJudgeList(int numG, int numMeet)
    {
        List<Judge_Group_Meeting> listJ = new List<Judge_Group_Meeting>();
        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = @"select u.* , j.sumScore from FeedBack_Meet m inner join FeedBack_Meet_Groups f 
                               on m.NumMeeting = f.NumMeeting inner join Groups g on g.NumGroup = f.NumGroup 
                               inner join Judge_Groups j on j.NumGroup = g.NumGroup inner join Users u 
                               on u.Email = j.EmailJudge where g.NumGroup = '" + numG + "' and m.NumMeeting = '" + numMeet + "'";


            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                Judge_Group_Meeting jg = new Judge_Group_Meeting();
                jg.Judge = new Users();

                jg.Judge.FirstName = (string)dr["FirstName"];
                jg.Judge.LastName = (string)dr["LastName"];
                jg.Judge.Email = (string)dr["Email"];
                jg.SumScore = Convert.ToInt32(dr["sumScore"]);

                listJ.Add(jg);

            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return listJ;
    }
    //--------------------------------------------student----------------------------------------------------//
    public List<Group_Meeting> getStudentDocs(string id)
    {


        List<Group_Meeting> lm = new List<Group_Meeting>();

        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = @"select distinct m.yearMeeting, g.NameGroup,g.NameOrganization,g.NameProject,g.ProjectType,
                               c.NumCourse,d.NumDepartment,d.NameDepartment,c.NameCourse from FeedBack_Meet m 
                               inner join FeedBack_Meet_Groups f on m.NumMeeting = f.NumMeeting inner join Groups g 
                               on g.NumGroup = f.NumGroup inner join Course c on c.NumCourse = m.NumCourse inner join 
                               Department d on d.NumDepartment = m.NumDepartment inner join studentInGroup sg on 
                               sg.NumGroup = g.NumGroup inner join Student s on s.IDStudent = sg.IDStudent 
                               where s.IDStudent = '" + id + "'";


            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                Group_Meeting gm = new Group_Meeting();
                gm.Group = new Groups();
                gm.FeedBackMeet = new FeedBack_Meeting();
                gm.FeedBackMeet.DetailsCourseDep = new CoursesAndDepartment();



                gm.Group.NameGroup = (string)dr["NameGroup"];
                gm.Group.NameProject = (string)dr["NameProject"];
                gm.Group.NameOrganization = (string)dr["NameOrganization"];
                gm.Group.ProjectType = (string)dr["ProjectType"];

                gm.FeedBackMeet.DetailsCourseDep.NumDepartment = Convert.ToInt32(dr["NumDepartment"]);
                gm.FeedBackMeet.DetailsCourseDep.NumCourse = Convert.ToInt32(dr["NumCourse"]);
                gm.FeedBackMeet.DetailsCourseDep.NameDepartment = (string)dr["NameDepartment"];
                gm.FeedBackMeet.DetailsCourseDep.NameCourse = (string)dr["NameCourse"];
                gm.FeedBackMeet.YearMeeting = (string)dr["yearMeeting"];

                lm.Add(gm);

            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        foreach (var item in lm)
        {
            item.Group.ListStudent = findTeamGroup(id, item.Group.NameGroup);
        }

        return lm;
    }
    public List<Students> findTeamGroup(string id, string nameG)
    {
        List<Students> ls = new List<Students>();

        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = @"select distinct s.FirstName ,s.LastNAme from  Groups g inner join studentInGroup sg on 
                               sg.NumGroup = g.NumGroup inner join Student s on s.IDStudent = sg.IDStudent 
                               where s.IDStudent <> '" + id + "' and g.NameGroup = '" + nameG + "'";


            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                Students s = new Students();
                s.FirstName = (string)dr["FirstName"];
                s.LastName = (string)dr["LastName"];

                ls.Add(s);
            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }

        return ls;
    }

    //--------------------------------get all doc for course---------------------------//
    public List<Group_Meeting> getStudentDocsMeet(string id, Group_Meeting selectedCourse)
    {


        List<Group_Meeting> doc = new List<Group_Meeting>();
        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = @"select u.FirstName,u.LastName,u.Email,m.DateMeeting,m.nameMetting,m.NumMeeting,g.finalScore,g.NumGroup ,f1.NumDoc from FeedBack_Meet m 
                              inner join FeedBack_Meet_Groups f on m.NumMeeting = f.NumMeeting inner join Groups g on g.NumGroup = f.NumGroup 
                              inner join Course c on c.NumCourse = m.NumCourse inner join Department d on d.NumDepartment = m.NumDepartment 
                              inner join studentInGroup sg on sg.NumGroup = g.NumGroup inner join Student s on s.IDStudent = sg.IDStudent 
                              inner join Users u on u.Email = g.EmailMentor  inner join FeedBack_Doc f1 on f1.NumMeeting=m.NumMeeting
                              where s.IDStudent = '" + id + "' and c.NumCourse = '" + selectedCourse.FeedBackMeet.DetailsCourseDep.NumCourse + "' and d.NumDepartment = '" + selectedCourse.FeedBackMeet.DetailsCourseDep.NumDepartment + "' and m.yearMeeting = '" + selectedCourse.FeedBackMeet.YearMeeting + "'";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                Group_Meeting gm = new Group_Meeting();
                gm.FeedBackMeet = new FeedBack_Meeting();


                gm.Group = new Groups();
                gm.Group.Mentor = new Users();

                gm.FeedBackMeet.NumMeeting = Convert.ToInt32(dr["NumMeeting"]);
                gm.FeedBackMeet.NumDoc = Convert.ToInt32(dr["NumDoc"]);
                gm.FeedBackMeet.NameMeeting = (string)dr["nameMetting"];
                gm.FeedBackMeet.Date = (string)dr["DateMeeting"];
                gm.Group.FinalScore = Convert.ToInt32(dr["finalScore"]);
                gm.Group.NumGroup = Convert.ToInt32(dr["NumGroup"]);
                gm.Group.Mentor.FirstName = (string)dr["FirstName"];
                gm.Group.Mentor.LastName = (string)dr["LastName"];
                gm.Group.Mentor.Email = (string)dr["Email"];



                doc.Add(gm);

            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        foreach (var item in doc)
        {
            item.JudgesGroup = new List<Judge_Group_Meeting>();
            item.JudgesGroup = findJudgeList(item.Group.NumGroup, item.FeedBackMeet.NumMeeting);
        }
        return doc;
    }

    //-----------------------------------------------lastDoc--------------------------------------//

    public List<CritInDoc> lastDoc(CoursesAndDepartment idCD)
    {

        List<CritInDoc> lastdoc = new List<CritInDoc>();
        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = @"select distinct d.NameDoc,m.yearMeeting ,d.NumDoc,d.totalW
                                from  FeedBack_Doc d inner join FeedBack_Meet m on m.NumMeeting=d.NumMeeting
                                where m.NumDepartment=" + idCD.NumDepartment + " and m.NumCourse=" + idCD.NumCourse + " and d.[status]=1";
            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                CritInDoc cd = new CritInDoc();
                cd.Doc = new FeedBack_Doc();
                cd.Doc.DetailsMeet = new FeedBack_Meeting();


                cd.Doc.NumDoc = Convert.ToInt32(dr["NumDoc"]);
                cd.Doc.NameDoc = (string)dr["NameDoc"];
                cd.Doc.TotalWeight = Convert.ToDouble(dr["totalW"]);
                cd.Doc.DetailsMeet.YearMeeting = (string)dr["yearMeeting"];




                lastdoc.Add(cd);

            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        foreach (var item in lastdoc)
        {
            CritInDoc temp = new CritInDoc();
            temp = get(item.Doc);
            item.AllCrit = temp.AllCrit;
        }
        return lastdoc;
    }
    //-------------------------------get all finalScore to Excel--------------------------//
    public List<Students> getFinalScore(int idDoc)
    {
        List<Students> listS = new List<Students>();
        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = @"  select s.FirstName,s.LastNAme ,s.IDStudent ,g.finalScore 
                                   from FeedBack_Meet  m inner join FeedBack_Doc d on 
                                   m.NumMeeting=d.NumMeeting   inner join Course c on c.NumCourse=m.NumCourse
                                   inner join Department de on de.NumDepartment=m.NumDepartment
                                   inner join FeedBack_Meet_Groups mg 
                                   on mg.NumMeeting=m.NumMeeting inner join Groups g on g.NumGroup=mg.NumGroup
                                   inner join studentInGroup sg on sg.NumGroup=g.NumGroup
                                   inner join Student s on s.IDStudent=sg.IDStudent
                                    where d.NumDoc='" + idDoc + "'";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                Students s = new Students();

                s.FirstName = (string)dr["FirstName"];
                s.LastName = (string)dr["LastName"];
                s.Id = (string)dr["IDStudent"];
                s.FinalScore = Convert.ToInt32(dr["finalScore"]);

                listS.Add(s);
            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }

        return listS;
    }

    //---------------------saveLink----------------------------------------//
    public int savelink(string link, int idGroup)
    {

        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;


        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }


        String cStr = BuildInsertCommandupda(link, idGroup);
        cmd = CreateCommand(cStr, con);

        try
        {
            numEffected += cmd.ExecuteNonQuery(); // execute the command


        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        return numEffected;
    }
    private String BuildInsertCommandupda(string link, int id)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string

        String prefix = "UPDATE [Groups] Set [linkPP] = '" + link + "'  where [NumGroup] = '" + id + "'";
        command = prefix + sb.ToString();

        return command;

    }
    public DBservices readFromDB()
    {
        SqlConnection con = null;

        try
        {
            con = connect("con15");
            da = new SqlDataAdapter("select * from Criteria", con);
            SqlCommandBuilder builder = new SqlCommandBuilder(da);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dt = ds.Tables[0];

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        return this;
    }
    public void update()
    {
        da.Update(dt);
    }

    //updateUserDetails
    public int updateUserDetails(Users u)
    {

        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }


        String cStr = BuildInsertCommandupdateFull(u);
        cmd = CreateCommand(cStr, con);


        try
        {
            numEffected += cmd.ExecuteNonQuery(); // execute the command


        }
        catch (Exception ex)
        {
            throw (ex);
        }



        if (con != null)
        {
            // close the db connection
            con.Close();
        }

        return numEffected;
    }
    //-------------------updateScoreForGroup----------------------------------//
    private String BuildInsertCommandupdateFull(Users u)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string

        String prefix = "UPDATE [Users] Set [FirstName] = '" + u.FirstName + "', [LastName]='" + u.LastName + "' , [Email]='" + u.Updateemail + "' ,[pass]='" + u.Pass + "' where [Email] = '" + u.Email + "'";
        command = prefix + sb.ToString();

        return command;

    }
    //-----------------updateStudentDetails--------------------------//
    public int updateStudentDetails(Students u)
    {

        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }


        String cStr = BuildInsertCommandupdateFull(u);
        cmd = CreateCommand(cStr, con);


        try
        {
            numEffected += cmd.ExecuteNonQuery(); // execute the command


        }
        catch (Exception ex)
        {
            throw (ex);
        }



        if (con != null)
        {
            // close the db connection
            con.Close();
        }

        return numEffected;
    }
    //-------------------updateScoreForGroup----------------------------------//
    private String BuildInsertCommandupdateFull(Students u)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string

        String prefix = "UPDATE [Student] Set [FirstName] = '" + u.FirstName + "', [LastName]='" + u.LastName + "' , [Email]='" + u.Email + "' ,[pass]='" + u.Pass + "' where [IDStudent] = '" + u.Id + "'";
        command = prefix + sb.ToString();

        return command;

    }

    //======================================calendar=======================================//
    //-----------------------by mentor---------------------------------------------------//
    public List<Groups> getmentorG(Users u)
    {
        List<Groups> list = new List<Groups>();
        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = @"select distinct NameGroup, NameProject from Groups
                                 where EmailMentor = '"+u.Email+"'";
            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                Groups g = new Groups();
          
               // g.NumGroup = Convert.ToInt32(dr["NumGroup"]);
                g.NameProject = (string)dr["NameProject"];
                g.NameGroup= (string)dr["NameGroup"];

                list.Add(g);
            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }

        foreach (var item in list)
        {
            item.NumGroup = findkeyGroup(item);
            item.ListStudent = foundStudents(item);
        }

        return list;
       
    }
    public List<calendarMeeting> GetCalendarMeetings(Users u)
    {
        List<calendarMeeting> list = new List<calendarMeeting>();
        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = @"  select*from calendarMeeting cm inner join Groups g on cm.NumGroup=g.NumGroup
                                   where cm.email='"+u.Email+"' and cm.utID='"+u.Type.NumType+"'";
            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                calendarMeeting cm = new calendarMeeting();
                cm.Group = new Groups();
                cm.Group.ListStudent = new List<Students>();
                
                cm.Id= Convert.ToInt32(dr["meetingID"]);
                cm.DateSelected = (string)dr["dateM"];
                cm.StartTime= (string)dr["starttime"];
                cm.EndTime= (string)dr["endtime"];  
                cm.PlaceMetting= (string)dr["placeMetting"];

                cm.Group.NumGroup= Convert.ToInt32(dr["NumGroup"]);
                cm.Group.NameProject= (string)dr["NameProject"];

                list.Add(cm);
            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }

        foreach (var item in list)
        {
            item.Group.ListStudent = foundStudents(item.Group);
        }

        return list;

    }
    public List<calendarTask> GetCalendarTask(Users u)
    {
        List<calendarTask> list = new List<calendarTask>();
        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = @"  select*from [calendarTasks] cm inner join Groups g on cm.NumGroup=g.NumGroup
                                   where cm.email='" + u.Email + "' and cm.utID='" + u.Type.NumType + "'";
            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                calendarTask cm = new calendarTask();
                cm.Group = new Groups();
                cm.Student = new Students();

                
                cm.Id = Convert.ToInt32(dr["TasksID"]);
                cm.Student.Id = (string)dr["IDStudent"];
                cm.NameTask = (string)dr["nameTask"];
                cm.Complet = (string)dr["taskcompletiondate"];
                cm.Startdate= (string)dr["dateM"];
                cm.Desc= (string)dr["descr"];
                cm.Status= Convert.ToInt32(dr["statusT"]);

                cm.Group.NumGroup = Convert.ToInt32(dr["NumGroup"]);
                cm.Group.NameGroup = (string)dr["NameGroup"];
                cm.Group.NameProject = (string)dr["NameProject"];

                list.Add(cm);
            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        //להביא פרטי סטודנט
        foreach (var item in list)
        {
            item.Group.ListStudent = foundStudents(item.Group);
        }
        return list;

    }
    //--------------------------insert calendar meeting----------------------//
    public int insert(calendarMeeting cm)
    {
        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;
     
        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

            String cStr = BuildInsertCommand(cm);
            cmd = CreateCommand(cStr, con);


            try
            {
                numEffected += cmd.ExecuteNonQuery(); // execute the command


            }
            catch (Exception ex)
            {
                throw (ex);
            }

        

       
        if (con != null)
        {
            // close the db connection
            con.Close();
        }
       
        return numEffected;
    }
    //-------------------insert calenderMeet----------------//
    private String BuildInsertCommand(calendarMeeting cm)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string
        sb.AppendFormat("Values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}')",
       cm.Mentor.Email,cm.Mentor.Type.NumType,cm.Group.NumGroup, cm.StartTime, cm.EndTime, cm.DateSelected,cm.PlaceMetting);
        String prefix = "INSERT INTO calendarMeeting " + "(Email,utID,NumGroup,starttime,endtime,dateM,placeMetting)";
        command = prefix + sb.ToString();

        return command;

    }
    //--------------------------insert calendar meeting----------------------//
    public int insert(calendarTask ct)
    {
        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        String cStr = BuildInsertCommand(ct);
        cmd = CreateCommand(cStr, con);


        try
        {
            numEffected += cmd.ExecuteNonQuery(); // execute the command


        }
        catch (Exception ex)
        {
            throw (ex);
        }




        if (con != null)
        {
            // close the db connection
            con.Close();
        }

        return numEffected;
    }
    //-------------------insert calenderMeet----------------//
    private String BuildInsertCommand(calendarTask cm)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string
        sb.AppendFormat("Values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}')",
       cm.Mentor.Email, cm.Mentor.Type.NumType, cm.Group.NumGroup, cm.Student.Id, cm.Complet, cm.NameTask,2,cm.Desc,cm.Startdate);
        String prefix = "INSERT INTO calendarTasks " + "(Email,utID,NumGroup,IDStudent,taskcompletiondate,nameTask,statusT,descr,dateM)";
        command = prefix + sb.ToString();

        return command;

    }
    //updateM
    //-----------------updatecalendarMeeting--------------------------//
    public int updateM(calendarMeeting cm)
    {

        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }


        String cStr = BuildInsertCommandupdate(cm);
        cmd = CreateCommand(cStr, con);


        try
        {
            numEffected += cmd.ExecuteNonQuery(); // execute the command


        }
        catch (Exception ex)
        {
            throw (ex);
        }



        if (con != null)
        {
            // close the db connection
            con.Close();
        }

        return numEffected;
    }
    //-------------------update----------------------------------//
    private String BuildInsertCommandupdate(calendarMeeting cm)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string

        String prefix = "UPDATE [calendarMeeting] Set [placeMetting] = '" + cm.PlaceMetting + "', [starttime]='" + cm.StartTime + "' , [endtime]='" + cm.EndTime + "' where [meetingID] = '" + cm.Id + "'";
        command = prefix + sb.ToString();

        return command;

    }
    //-----------------updatecalendarMeeting--------------------------//
    public int updateT(calendarTask ct)
    {

        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }


        String cStr = BuildInsertCommandupdate(ct);
        cmd = CreateCommand(cStr, con);


        try
        {
            numEffected += cmd.ExecuteNonQuery(); // execute the command


        }
        catch (Exception ex)
        {
            throw (ex);
        }



        if (con != null)
        {
            // close the db connection
            con.Close();
        }

        return numEffected;
    }
    //-------------------update----------------------------------//
    private String BuildInsertCommandupdate(calendarTask cm)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string

        String prefix = "UPDATE [calendarTasks] Set [dateM]='"+cm.Startdate+"' , [taskcompletiondate] = '" + cm.Complet + "',[nameTask]='" + cm.NameTask + "' , [IDStudent]='" + cm.Student.Id + "' , [descr]='"+cm.Desc+"' where [TasksID] = '" + cm.Id + "'";
        command = prefix + sb.ToString();

        return command;

    }
    public int deleteT(int id)
    {
        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }


        StringBuilder sb = new StringBuilder();


        sb.AppendFormat("delete from calendarTasks where TasksID ={0}", id);
        String cStr = sb.ToString();
        // helper method to build the insert string

        cmd = CreateCommand(cStr, con);             // create the command

        try
        {
            numEffected += cmd.ExecuteNonQuery(); // execute the command

        }
        catch (Exception ex)
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }


        return numEffected;

    }
    public int deleteM(int id)
    {
        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }


        StringBuilder sb = new StringBuilder();


        sb.AppendFormat("delete from [calendarMeeting] where [meetingID] ={0}", id);
        String cStr = sb.ToString();
        // helper method to build the insert string

        cmd = CreateCommand(cStr, con);             // create the command

        try
        {
            numEffected += cmd.ExecuteNonQuery(); // execute the command

        }
        catch (Exception ex)
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }


        return numEffected;

    }
    //-----------------calander Student----------------------//
    public List<calendarMeeting> GetCalendarMeetingsStudent(string idStudent)
    {
        List<calendarMeeting> list = new List<calendarMeeting>();
        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = @"select cm.* ,g.NameGroup,g.NameProject,g.EmailMentor,u.FirstName,u.LastName 
                                 from calendarMeeting cm inner join Groups g 
                                 on cm.NumGroup=g.NumGroup inner join studentInGroup sg
                                 on g.NumGroup=sg.NumGroup inner join Student s on s.IDStudent=sg.IDStudent
                                 inner join Users u on u.Email=g.EmailMentor
                                 where s.IDStudent='"+idStudent+"'";
            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                calendarMeeting cm = new calendarMeeting();
                cm.Group = new Groups();
                cm.Group.ListStudent = new List<Students>();
                cm.Mentor = new Users();

                cm.Id = Convert.ToInt32(dr["meetingID"]);
                cm.DateSelected = (string)dr["dateM"];
                cm.StartTime = (string)dr["starttime"];
                cm.EndTime = (string)dr["endtime"];
                cm.PlaceMetting = (string)dr["placeMetting"];

                cm.Group.NumGroup = Convert.ToInt32(dr["NumGroup"]);
                cm.Group.NameProject = (string)dr["NameProject"];
                cm.Mentor.Email= (string)dr["EmailMentor"];
                cm.Mentor.FirstName= (string)dr["FirstName"];
                cm.Mentor.LastName = (string)dr["LastName"];
                list.Add(cm);
            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }

        foreach (var item in list)
        {
            item.Group.ListStudent = foundStudents(item.Group);
        }

        return list;

    }
    public List<calendarTask> GetCalendarTaskStudent(string idStudent)
    {
        List<calendarTask> list = new List<calendarTask>();
        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = @"select * from calendarTasks ct inner join Groups g on ct.NumGroup=g.NumGroup where IDStudent='" + idStudent+"'";
            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                calendarTask cm = new calendarTask();
                cm.Group = new Groups();
                cm.Student = new Students();


                cm.Id = Convert.ToInt32(dr["TasksID"]);
                cm.Student.Id = (string)dr["IDStudent"];
                cm.NameTask = (string)dr["nameTask"];
                cm.Complet = (string)dr["taskcompletiondate"];
                cm.Startdate = (string)dr["dateM"];
                cm.Desc = (string)dr["descr"];
                cm.Status = Convert.ToInt32(dr["statusT"]);

                cm.Group.NumGroup = Convert.ToInt32(dr["NumGroup"]);
                cm.Group.NameGroup = (string)dr["NameGroup"];
                cm.Group.NameProject = (string)dr["NameProject"];

                list.Add(cm);
            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        ////להביא פרטי סטודנט
        //foreach (var item in list)
        //{
        //    item.Group.ListStudent = foundStudents(item.Group);
        //}
        return list;

    }

    //-----------get all mentors---------------------//
    public List<Users> getAllMentor(string idStudent)
    {
        List<Users> list = new List<Users>();
        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = @"select distinct u.* from Users u inner join Groups g 
                                 on u.Email=g.EmailMentor inner join studentInGroup sg
                                 on sg.NumGroup=g.NumGroup inner join Student s on s.IDStudent=sg.IDStudent
                                 where s.IDStudent='" + idStudent + "'";
            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                Users u = new Users();
              u.FirstName= (string)dr["FirstName"];
                u.LastName= (string)dr["LastName"];
                u.Email= (string)dr["Email"];

                list.Add(u);
            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
      
        return list;

    }
    //-----------------updatecalendarMeeting--------------------------//
    public int updateStatust(int idTask)
    {

        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }


        String cStr = BuildInsertCommandupdate(idTask);
        cmd = CreateCommand(cStr, con);


        try
        {
            numEffected += cmd.ExecuteNonQuery(); // execute the command


        }
        catch (Exception ex)
        {
            throw (ex);
        }



        if (con != null)
        {
            // close the db connection
            con.Close();
        }

        return numEffected;
    }
    //-------------------update----------------------------------//
    private String BuildInsertCommandupdate(int idTask)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string

        String prefix = "UPDATE [calendarTasks] Set [statusT]='" + 1 + "' where [TasksID] = '" + idTask + "'";
        command = prefix + sb.ToString();
                return command;

    }
    //---------------------get all projmentor by student--------------------

    public List<Groups> getAllMentorProg(string idStudent)
    {
        List<Groups> list = new List<Groups>();
        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = @"select distinct g.NameGroup,g.NameProject , g.EmailMentor from Groups g inner join 
                                 studentInGroup sg on sg.NumGroup=g.NumGroup inner join Student s on s.IDStudent= sg.IDStudent
                                 where s.IDStudent='" + idStudent + "'";
            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                Groups g = new Groups();
                g.Mentor = new Users();
               
                g.NameGroup= (string)dr["NameGroup"];
                g.NameProject= (string)dr["NameProject"];
                g.Mentor.Email = (string)dr["EmailMentor"];

                list.Add(g);
            }

        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }

        }
        foreach (var item in list)
        {
            item.NumGroup = findkeyGroup(item);
        }
        foreach (var item in list)
        {
            item.ListStudent = foundStudents(item);
        }
        return list;

    }
    public int update(Group_Meeting gm)
    {


        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;
        SqlCommand cmd1;
        try
        {
            con = connect("con15"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }


        String cStr = BuildInsertCommandupdate(gm.Group);
        cmd = CreateCommand(cStr, con);
        try
        {
            numEffected += cmd.ExecuteNonQuery(); // execute the command


        }
        catch (Exception ex)
        {
            throw (ex);
        }
        String cStr1 = BuildInsertCommandupdate(gm);
        cmd1 = CreateCommand(cStr1, con);
        try
        {
            numEffected += cmd1.ExecuteNonQuery(); // execute the command


        }
        catch (Exception ex)
        {
            throw (ex);
        }



        if (con != null)
        {
            // close the db connection
            con.Close();
        }

        return numEffected;
    }
    //-------------------update----------------------------------//
    private String BuildInsertCommandupdate(Groups cm)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string

        String prefix = "UPDATE [Groups] Set [NameProject]='" + cm.NameProject + "' , [NameOrganization] = '" + cm.NameOrganization + "',[ProjectType]='" + cm.ProjectType + "'  where [NameGroup] = '" + cm.NameGroup + "' and [NameProject]='" + cm.NameProject + "' and [NameOrganization]='"+cm.BeforeLastOrg+"'";
        command = prefix + sb.ToString();

        return command;

    }
    private String BuildInsertCommandupdate(Group_Meeting cm)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string

        String prefix = "UPDATE [Judge_Groups] Set [startTime]='" + cm.StartTime + "' , [endTime] = '" + cm.EndTime + "'  where [NumGroup] = '" + cm.Group.NumGroup + "'";
        command = prefix + sb.ToString();

        return command;

    }

   
}

