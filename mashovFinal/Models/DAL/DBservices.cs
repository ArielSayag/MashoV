using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;
using System.Text;
using mashovFinal.Models;

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
    // This method deletes a car to the movie table CLASSEX
    //--------------------------------------------------------------------------------------------------

    //public int delete(int id)
    //{

    //    SqlConnection con;
    //    SqlCommand cmd;

    //    try
    //    {
    //        con = connect("carsDBConnectionString"); // create the connection
    //    }
    //    catch (Exception ex)
    //    {
    //        // write to log
    //        throw (ex);
    //    }

    //    String cStr = BuildDeleteCommand(id);      // helper method to build the insert string

    //    cmd = CreateCommand(cStr, con);             // create the command

    //    try
    //    {
    //        int numEffected = cmd.ExecuteNonQuery(); // execute the command
    //        return numEffected;
    //    }
    //    catch (Exception ex)
    //    {
    //        return 0;
    //        // write to log
    //        throw (ex);
    //    }

    //    finally
    //    {
    //        if (con != null)
    //        {
    //            // close the db connection
    //            con.Close();
    //        }
    //    }

    //}



    //--------------------------------------------------------------------------------------------------
    // This method inserts a car to the movie table CLASSEX
    //--------------------------------------------------------------------------------------------------
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
          user.Email, user.FirstName, user.LastName, "");
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
        s.Id, s.FirstName, s.LastName, s.Email, "");
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
        sb.AppendFormat("Values('{0}', '{1}','{2}', '{3}','{4}','{5}')",
        g.NameGroup, g.NameProject, g.NameOrganization, g.ProjectType, g.Mentor.Email, g.Mentor.Type.NumType);
        String prefix = "INSERT INTO Groups " + "(NameGroup,NameProject,NameOrganization,ProjectType,EmailMentor,utID)";
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
        sb.AppendFormat("Values('{0}', '{1}','{2}','{3}','{4}')",
        EJudge, gm.Group.NumGroup, 3, gm.StartTime, gm.EndTime);
        String prefix = "INSERT INTO Judge_Groups " + "(EmailJudge,NumGroup,utID,startTime,endTime)";
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
        sb.AppendFormat("Values('{0}', '{1}')",
        d.NameDoc, d.DetailsMeet.NumMeeting);
        String prefix = "INSERT INTO FeedBack_Doc " + "(NameDoc,NumMeeting)";
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
            item.NumCrit=findkeyCrit(item);

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
        sb.AppendFormat("Values('{0}', '{1}')",
        c.NameCrit, c.DescriptionCrit);
        String prefix = "INSERT INTO Criteria " + "(NameCrit,DescriptionCrit)";
        command = prefix + sb.ToString();

        return command;

    }
    //----------insertCritINDoc-------------------//
    public int insert(CritInDoc fullDoc)
    {
        fullDoc.AllCrit = insert(fullDoc.AllCrit);
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



        String cStr = BuildInsertCommandupdate( fullDoc.Doc);
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
        c.NumCrit, numDoc,c.WeightCrit,c.TypeCrit);
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
        sb.AppendFormat("Values('{0}')",f.Status);
        String prefix = "UPDATE FeedBack_Doc Set [status] = '"+f.Status+"' , totalW='"+f.TotalWeight+"'  where[NumDoc] = '"+f.NumDoc+"'";
        command = prefix + sb.ToString();

        return command;

    }
    //----------insertAllCritwithAllScoreOfJudge-------------------//
    public int insert(FullFeedback full)
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

        foreach (var item in full.CritDoc.AllCrit)
        {
            foreach (var tem in full.GroupM.JudgesGroup)
            {
                String cStr = BuildInsertCommand(full, item, tem);
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
        }

        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        return numEffected;
    }
    //-------------------insertAllCritwithAllScoreOfJudge----------------//
    private String BuildInsertCommand(FullFeedback full, Criterion c, Judge_Group_Meeting j)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string
        sb.AppendFormat("Values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')",
       c.NumCrit, full.CritDoc.Doc.NumDoc, j.Judge.Email, full.GroupM.Group.NumGroup, c.Score, c.Note);
        String prefix = "INSERT INTO Full_feedback " + "(NumCrit,NumDoc,EmailJudge,NumGroup,Score,Note)";
        command = prefix + sb.ToString();

        return command;

    }





    //----------------------------------------------------------------------------------------------//
    //--------------------------------------------Login Page ---------------------------------------//
    //----------------------------------------------------------------------------------------------//


    public List<Users> checkingExistUser(Users u)
    {

        List<Users> user = new List<Users>();

        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = "select u.FirstName , u.LastName ,u.Email ,u.pass ,tu.utID , t.TypeUser as TypeUser  from Users u inner join UsersType tu  on u.Email=tu.Email inner join [Type] t on tu.utID=t.utID where u.Email='" + u.Email + "'";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                Users us = new Users();
                us.Type = new Types();
                us.Email = (string)dr["Email"];
                us.Pass = (string)dr["pass"];
                us.FirstName = (string)dr["FirstName"];
                us.LastName = (string)dr["LastName"];
                us.Type.Type = (string)dr["TypeUser"];
                us.Type.NumType = Convert.ToInt32(dr["utID"]);

                user.Add(us);

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
            String selectSTR = " select * from Users where IDStudent='" + u.Id + "'";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                s.Id = (string)dr["IDStudent"];
                s.Pass = (string)dr["pass"];
                s.FirstName = (string)dr["FirstName"];
                s.LastName = (string)dr["LastName"];

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


        String prefix = " UPDATE [Student] SET pass = '" + s.Pass + "' WHERE Email = '" + s.Id + "'";
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
            String selectSTR = "  select * from FeedBack_Doc d inner join FeedBack_Meet m on d.NumMeeting=m.NumMeeting inner join Manager_feedback a on a.NumDoc = d.NumDoc inner join Course c on c.NumCourse = m.NumCourse inner join Department de on de.NumDepartment = m.NumDepartment where a.EmailManager='" + u.Email + "' and a.utID='" + u.Type.NumType + "'";

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
                d.DetailsMeet.Date = (DateTime)dr["DateMeeting"];
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
            
            String selectSTR = "select cr.NumCrit,cr.NameCrit,cr.DescriptionCrit ,c.WeightCrit,c.numScala ,s.nameScala from FeedBack_Doc d inner join[dbo].[FeedBack_Criteria] c on d.NumDoc=c.NumDoc inner join[dbo].[Criteria] cr on cr.NumCrit=c.NumCrit inner join Scala s on c.numScala=s.numScala where d.NumDoc='" + d.NumDoc + "'";

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
            String selectSTR = "select NumDoc, NameDoc, NumMeeting, [status] from FeedBack_Doc where NumDoc = (select max([NumDoc]) from FeedBack_Doc)";

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
            String selectSTR = "select g.NumGroup, g.NameGroup , g.NameProject,jg.startTime , jg.endTime from Groups g inner join[dbo].[FeedBack_Meet_Groups] n on g.NumGroup=n.NumGroup inner join [dbo].[FeedBack_Meet] m on n.NumMeeting=m.NumMeeting inner join[dbo].[FeedBack_Doc] d on d.NumMeeting=m.NumMeeting inner join Judge_Groups jg on jg.NumGroup=g.NumGroup where d.NumDoc='" + numMeet+"'";
            
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
                st.LastName = (string)dr["LastName"];
                g.NumGroup= Convert.ToInt32(dr["NumGroup"]);
                g.NameProject= (string)dr["NameProject"];


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
       List<int> oldNumG= checkednameProject(s);
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

                String cStr1 = BuildInsertCommandupdate(item,newNumG[j],s.Id);//updet student in group 
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
    private String BuildInsertCommandupdate(int oldNumGroup,int newNymGroup ,string idOfSelectedStudent)
    {
        String command;
        StringBuilder sb = new StringBuilder();
        String prefix = "update studentInGroup set NumGroup='"+ newNymGroup + "'where IDStudent='"+ idOfSelectedStudent + "'and NumGroup='"+ oldNumGroup + "'";
        command = prefix + sb.ToString();

        return command;
    }
    //------------get all old Id groups for student -------------------------//
    private List<int> checkednameProject(Students  s)
    {
        List<int> listOldIDG = new List<int>();

        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = "select g.NumGroup  from Groups g inner join studentInGroup sg on g.NumGroup=sg.NumGroup inner join Student s on s.IDStudent=sg.IDStudent where s.IDStudent='"+s.Id+"' and g.NameProject='" + s.GroupS[0].NameProject+"'";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


            while (dr.Read())
            {
                int old= Convert.ToInt32(dr["NumGroup"]);

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
            String selectSTR = " select distinct  g.NumGroup from Groups g inner join studentInGroup sg on g.NumGroup = sg.NumGroupwhere g.NameOrganization = '"+s.GroupS[1].NameProject+"'";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


            while (dr.Read())
            {
                int newid= Convert.ToInt32(dr["NumGroup"]);


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
    public int insAndUpCrit(CritInDoc c)
    {
        int numEffected = 0;
        SqlConnection con;
        SqlCommand cmd;
        int newID ;

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
            newID = findkeyCrit(c.AllCrit[0]);
        }
        else
        {
            newID = c.AllCrit[0].NumCrit;
        }
        numEffected+= updateIDcritAndDoc(c,newID);
        return newID;
    }
    public bool existingCrit(Criterion c1)
    {
        bool yesOrNo = false;
        SqlConnection con = null;
        try
        {
            con = connect("con15");
            String selectSTR = "select * from Criteria where NameCrit='"+c1.NameCrit+"' and DescriptionCrit='"+c1.DescriptionCrit+"'";

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
    public int updateIDcritAndDoc(CritInDoc cd,int id)
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

        String cStr = BuildInsertCommandUpdate(cd,id); // helper method to build the insert string
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
    private String BuildInsertCommandUpdate(CritInDoc cd,int id)
    {
        String command;

        StringBuilder sb = new StringBuilder();
        String prefix = "UPDATE FeedBack_Criteria Set [NumCrit] = '" + id + "' , [WeightCrit]='"+cd.AllCrit[0].WeightCrit+"' , [numScala]='"+cd.AllCrit[0].TypeCrit+"' where [NumDoc]='" + cd.Doc.NumDoc + "' AND [NumCrit]='"+cd.AllCrit[0].NumCrit+"'";
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
            String selectSTR = "select * from Criteria ";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


            while (dr.Read())
            {
                Criterion c = new Criterion();
                c.NumCrit = Convert.ToInt32(dr["NumCrit"]);
                c.NameCrit = (string)dr["NameCrit"];
                //c.WeightCrit = Convert.ToDouble(dr["WeightCrit"]);
                //c.TypeCrit = Convert.ToInt32(dr["numScala"]);
                //c.NameScala = (string)dr["nameScala"];
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
            String selectSTR = " select distinct m.NumMeeting, m.DateMeeting,m.yearMeeting,m.nameMetting,d.NameDepartment,c.NameCourse from Judge_Groups jg inner join FeedBack_Meet_Groups mg on jg.NumGroup=mg.NumGroup inner join FeedBack_Meet m on m.NumMeeting=mg.NumMeeting inner join Course c on c.NumCourse=m.NumCourse inner join Department d on d.NumDepartment=m.NumDepartment where jg.EmailJudge='" + u.Email+"' and jg.utID='"+u.Type.NumType+"'";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {

                FeedBack_Meeting DetailsMeet = new FeedBack_Meeting();
                DetailsMeet.DetailsCourseDep = new CoursesAndDepartment();
            
                DetailsMeet.NumMeeting = Convert.ToInt32(dr["NumMeeting"]);
                DetailsMeet.NameMeeting = (string)dr["nameMetting"];
                DetailsMeet.YearMeeting = (string)dr["yearMeeting"];
                DetailsMeet.DetailsCourseDep.NumCourse = Convert.ToInt32(dr["NumCourse"]);
                DetailsMeet.DetailsCourseDep.NumDepartment = Convert.ToInt32(dr["NumDepartment"]);
                DetailsMeet.DetailsCourseDep.NameDepartment = (string)dr["NameDepartment"];
                DetailsMeet.DetailsCourseDep.NameCourse = (string)dr["NameCourse"];
                DetailsMeet.Date = (DateTime)dr["DateMeeting"];
          

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
    public List<Group_Meeting> getJudgeGroups(Users u)//??
    {


        List<Group_Meeting> allGroup = new List<Group_Meeting>();
        SqlConnection con = null;

        try
        {
            con = connect("con15");
            String selectSTR = "select distinct g.NumGroup,g.NameGroup,g.NameProject,g.NameOrganization ,u.FirstName,u.LastName,jg.startTime,jg.endTime  from Groups g inner join Judge_Groups jg on g.NumGroup=jg.NumGroup inner join  FeedBack_Meet_Groups mg on mg.NumGroup = jg.NumGroup inner join FeedBack_Meet m on m.NumMeeting = mg.NumMeeting inner join Users u on u.Email = g.EmailMentor where jg.EmailJudge = '" + u.Email+"' and mg.NumMeeting = ''";


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
     
       
        return foundStudents(allGroup);
    }
    //---------------------------getAllStudentforGroup--------------------------//
    public List<Group_Meeting> foundStudents(List<Group_Meeting> all)
    {
        // List<Group_Meeting> allGroup = new List<Group_Meeting>();
        foreach (var g in all)
        {
            SqlConnection con = null;
            g.Group.ListStudent = new List<Students>();

            try
            {
                con = connect("con15");
                String selectSTR = "select s.FirstName,s.LastNAme,s.IDStudent from Groups g inner join studentInGroup sg on g.NumGroup = sg.NumGroup  inner join Student s on s.IDStudent = sg.IDStudent inner join  FeedBack_Meet_Groups mg on mg.NumGroup = g.NumGroup where g.NameGroup = '" + g.Group.NumGroup + "'";

                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dr.Read())
                {
                    Students s = new Students();

                    s.FirstName = (string)dr["FirstName"];
                    s.LastName = (string)dr["LastNAme"];
                    s.Id = (string)dr["IDStudent"];

                    g.Group.ListStudent.Add(s);




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
        }
       
        return all;
    }
}


