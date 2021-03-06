using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace HairSalon
{
    public class Stylist
    {
        private int _id;
        private string _name;

        public Stylist(string name, int id = 0)
        {
            _name = name;
            _id = id;
        }

        public override bool Equals(System.Object otherStylist)
        {
            if(!(otherStylist is Stylist))
            {
                return false;
            }
            else
            {
                Stylist newStylist = (Stylist) otherStylist;
                bool idEquality = this.GetId() == newStylist.GetId();
                bool nameEquality = this.GetName() == newStylist.GetName();
                return(idEquality && nameEquality);
            }
        }

        public override int GetHashCode()
        {
            return this.GetName().GetHashCode();
        }

        public static List<Stylist> GetAll()
        {
            List<Stylist> allStylists = new List<Stylist>{};

            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM stylists;", conn);

            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                int stylistId = rdr.GetInt32(0);
                string stylistName = rdr.GetString(1);
                Stylist newStylist = new Stylist(stylistName, stylistId);
                allStylists.Add(newStylist);
            }

            DB.CloseSqlConnections(rdr, conn);

            return allStylists;
        }

        public void Save()
        {
            if(IsNewStylist(this.GetName()) == -1)
            {
                SqlConnection conn = DB.Connection();
                conn.Open();

                SqlCommand cmd = new SqlCommand("INSERT INTO stylists (name) OUTPUT INSERTED.id VALUES (@StylistName);", conn);

                cmd.Parameters.Add(new SqlParameter("@StylistName", this.GetName()));
                SqlDataReader rdr = cmd.ExecuteReader();

                while(rdr.Read())
                {
                    this._id = rdr.GetInt32(0);
                }

                DB.CloseSqlConnections(rdr, conn);
            }
        }

        public static int IsNewStylist(string targetName)
        {
            int result;

            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM stylists WHERE name = @TargetName;", conn);
            cmd.Parameters.Add(new SqlParameter("@TargetName", targetName));
            SqlDataReader rdr = cmd.ExecuteReader();

            if(rdr.Read())
            {
                result = rdr.GetInt32(0);
            }
            else{
                result = -1;
            }

            DB.CloseSqlConnections(rdr, conn);

            return result;
        }

        public static Stylist Find(int id)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM stylists WHERE id = @StylistId;", conn);
            cmd.Parameters.Add(new SqlParameter("@StylistId", id));
            SqlDataReader rdr = cmd.ExecuteReader();

            int foundId = 0;
            string foundName = null;

            while(rdr.Read())
            {
                foundId = rdr.GetInt32(0);
                foundName = rdr.GetString(1);
            }

            Stylist foundStylist = new Stylist(foundName, foundId);

            DB.CloseSqlConnections(rdr, conn);

            return foundStylist;
        }

        public static Stylist FindByName(string name)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM stylists WHERE name = @StylistName;", conn);
            cmd.Parameters.Add(new SqlParameter("@StylistName", name));
            SqlDataReader rdr = cmd.ExecuteReader();

            int foundId = 0;
            string foundName = null;

            while(rdr.Read())
            {
                foundId = rdr.GetInt32(0);
                foundName = rdr.GetString(1);
            }

            Stylist foundStylist = new Stylist(foundName, foundId);

            DB.CloseSqlConnections(rdr, conn);

            return foundStylist;
        }

        public List<Client> GetClients()
        {
            List<Client> stylistClients = new List<Client>{};

            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM clients WHERE stylist_id = @StylistId;", conn);
            cmd.Parameters.Add(new SqlParameter("@StylistId", this.GetId()));
            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                int foundId = rdr.GetInt32(0);
                string foundName = rdr.GetString(1);
                int foundStylistId = rdr.GetInt32(2);

                stylistClients.Add(new Client(foundName, foundStylistId, foundId));
            }

            DB.CloseSqlConnections(rdr, conn);

            return stylistClients;
        }

        public int GetId()
        {
            return _id;
        }
        public void SetId(int id)
        {
            _id = id;
        }
        public string GetName()
        {
            return _name;
        }
        public void SetName(string name)
        {
            _name = name;
        }

        public static void DeleteAll()
        {
            DB.TableDeleteAll("stylists");
        }

    }
}
