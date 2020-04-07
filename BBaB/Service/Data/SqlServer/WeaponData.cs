using BBaB.Models;
using BBaB.Service.CrudExceptions.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using BBaB.Service.Data;

namespace BBaB.Services.Data
{
    public class WeaponData : ICrud<WeaponModel>
    {
        private SqlConnection _connection;

        public WeaponData(SqlConnection connection)
        {
            this._connection = connection;
        }

        /**
        * <see cref="ICrud{T}"/>
        */
        public void CreateT(WeaponModel model)
        {
            try
            {

                //Create connection and command
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Genereate sql script into command
                    command.CommandText = @"INSERT INTO [bbab].[dbo].[Weapon] 
                    ([MAKE], [MODEL], [CALIBER], [SERIAL_NUMBER], [PRICE]) 
                    VALUES (@make, @model, @caliber, @serial, @price)";

                    //Add parameters to the command string
                    command.Parameters.Add("@make", SqlDbType.NVarChar, 50).Value = model._make;
                    command.Parameters.Add("@model", SqlDbType.NVarChar, 50).Value = model._model;
                    command.Parameters.Add("@caliber", SqlDbType.NVarChar, 10).Value = model._caliber;
                    command.Parameters.Add("@serial", SqlDbType.NVarChar, 25).Value = model._serialNumber;
                    command.Parameters.Add("@price", SqlDbType.Float).Value = model._price;

                    //Prepare the statement
                    command.Prepare();

                    //Execute the command
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw new RecordNotCreatedException("Record was not created, place try again or contact support.", e.InnerException);
            }
        }

        /**
        * <see cref="ICrud{T}"/>
        */
        public void DeleteT(WeaponModel model)
        {
            try
            {
                //Create the connection and command
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Generate sql script
                    command.CommandText = @"DELETE FROM [bbab].[dbo].[Weapon] where [WEAPON_ID] = @weaponid";

                    //Add parameters to command
                    command.Parameters.Add("@weaponid", SqlDbType.Int).Value = model._id;

                    //Prepare the statement
                    command.Prepare();

                    //Execute the statement
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw new RecordNotDeletedException("Weapon data not deleted.", e.InnerException);
            }
        }

        /**
        * <see cref="ICrud{T}"/>
        */
        public List<WeaponModel> ReadAllT()
        {
            try
            {
                //Create temp model to store the data
                List<WeaponModel> weapons;

                //Create the connection and command
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //write the sql script to the command
                    command.CommandText = @"select [WEAPON_ID], [MAKE], [MODEL], [CALIBER], [SERIAL_NUMBER], [PRICE]
                                        from [bbab].[dbo].[Weapon]";

                    //prepare the statement
                    command.Prepare();


                    //read the data recieved
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //Don't read if no rows were returned
                        if (reader.HasRows)
                        {
                            weapons = new List<WeaponModel>();
                            WeaponModel temp;
                            //Iterate through all the rows
                            while (reader.Read())
                            {
                                //Populate model and push onto List
                                temp = new WeaponModel();
                                temp._id = reader.GetInt32(0);
                                temp._make = reader.GetString(1);
                                temp._model = reader.GetString(2);
                                temp._caliber = reader.GetString(3);
                                temp._serialNumber = reader.GetString(4);
                                temp._price = reader.GetDouble(5);
                                

                                weapons.Add(temp);

                            }
                        }
                        else
                        {
                            reader.Close();
                            throw new RecordNotFoundException("No Weapons found to return.");
                        }

                        //Close the reader
                        reader.Close();
                    }
                }

                //return the model
                return weapons;

            }
            catch (Exception e)
            {
                throw new RecordNotFoundException("No Weapons found to return", e.InnerException);
            }
        }

        /**
        * <see cref="ICrud{T}"/>
        */
        public List<WeaponModel> ReadBetweenT(int low, int high)
        {
            try
            {
                //Create temp model to store the data
                List<WeaponModel> weapons;

                //Create the connection and command
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //write the sql script to the command
                    command.CommandText = @"select [WEAPON_ID], [MAKE], [MODEL], [CALIBER], [SERIAL_NUMBER], [PRICE]
                                        from [bbab].[dbo].[Weapon]
                                        where [WEAPON_ID] BETWEEN @low AND @high";

                    //add in parameters to the sql script
                    command.Parameters.Add("@low", SqlDbType.Int).Value = low;
                    command.Parameters.Add("@high", SqlDbType.Int).Value = high;

                    //prepare the statement
                    command.Prepare();


                    //read the data recieved
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //Don't read if no rows were returned
                        if (reader.HasRows)
                        {
                            weapons = new List<WeaponModel>();
                            WeaponModel temp;
                            //Iterate through all the rows
                            while (reader.Read())
                            {
                                //Populate model and push onto List
                                temp = new WeaponModel();
                                temp._id = reader.GetInt32(0);
                                temp._make = reader.GetString(1);
                                temp._model = reader.GetString(2);
                                temp._caliber = reader.GetString(3);
                                temp._serialNumber = reader.GetString(4);
                                temp._price = reader.GetFloat(5);


                                weapons.Add(temp);

                            }
                        }
                        else
                        {
                            reader.Close();
                            throw new RecordNotFoundException("No Weapons found to return.");
                        }

                        //Close the reader
                        reader.Close();
                    }
                }

                //return the model
                return weapons;

            }
            catch (Exception e)
            {
                throw new RecordNotFoundException("No weapons found to return", e.InnerException);
            }
        }

        /**
         * <remarks>Search by any combination of make, model, caliber, serial</remarks>
        * <see cref="ICrud{T}"/>
        */
        public WeaponModel ReadTByField(WeaponModel model)
        {
            try
            {

                //Create the connection and command
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //write the sql script to the command
                    command.CommandText = @"select [WEAPON_ID], [MAKE], [MODEL], [CALIBER], [SERIAL_NUMBER], [PRICE]
                                        from [bbab].[dbo].[Weapon]
                                        where [MAKE] LIKE @make
										OR [MODEL] LIKE @model
										OR [CALIBER] LIKE @caliber
										OR [SERIAL_NUMBER] LIKE @serial";

                    //add in parameters to the sql script
                    command.Parameters.Add("@make", SqlDbType.NVarChar, 50).Value = model._make;
                    command.Parameters.Add("@model", SqlDbType.NVarChar, 50).Value = model._model;
                    command.Parameters.Add("@caliber", SqlDbType.NVarChar, 10).Value = model._caliber;
                    command.Parameters.Add("@serial", SqlDbType.NVarChar, 25).Value = model._serialNumber;

                    //prepare the statement
                    command.Prepare();

                    //read the data recieved
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //Don't read if no rows were returned
                        if (reader.HasRows)
                        { 
                            //Iterate through all the rows
                            while (reader.Read())
                            {
                                //Populate model
                                model._id = reader.GetInt32(0);
                                model._make = reader.GetString(1);
                                model._model = reader.GetString(2);
                                model._caliber = reader.GetString(3);
                                model._serialNumber = reader.GetString(4);
                                model._price = reader.GetFloat(5);
                            }
                        }
                        else
                        {
                            reader.Close();
                            throw new RecordNotFoundException("No Weapons found to return.");
                        }

                        //Close the reader
                        reader.Close();
                    }
                }

                //return the model
                return model;

            }
            catch (Exception e)
            {
                throw new RecordNotFoundException("No weapon found to return.", e.InnerException);
            }
        }

        /**
        * <see cref="ICrud{T}"/>
        */
        public WeaponModel ReadTById(int id)
        {
            try
            {
                WeaponModel temp = new WeaponModel();
                //Create the connection and command
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //write the sql script to the command
                    command.CommandText = @"select [WEAPON_ID], [MAKE], [MODEL], [CALIBER], [SERIAL_NUMBER], [PRICE]
                                        from [bbab].[dbo].[Weapon]
                                        where [WEAPON_ID] = @weaponid";

                    //add in parameters to the sql script
                    command.Parameters.Add("@weaponid", SqlDbType.Int).Value = id;

                    //prepare the statement
                    command.Prepare();

                    //read the data recieved
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //Don't read if no rows were returned
                        if (reader.HasRows)
                        {
                            //Iterate through all the rows
                            while (reader.Read())
                            {
                                //Populate model
                                temp = new WeaponModel(reader.GetInt32(0),
                                    reader.GetString(1),
                                    reader.GetString(2),
                                    reader.GetString(3),
                                    reader.GetString(4),
                                    reader.GetFloat(5));
                            }
                        }
                        else
                        {
                            reader.Close();
                            throw new RecordNotFoundException("No Weapons found to return.");
                        }

                        //Close the reader
                        reader.Close();
                    }
                }

                //return the model
                return temp;

            }
            catch (Exception e)
            {
                throw new RecordNotFoundException("User not found", e.InnerException);
            }
        }

        /**
        * <see cref="ICrud{T}"/>
        */
        public void UpdateT(WeaponModel model)
        {
            try
            {
                //Create the connection and command
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Generate the command sql statement
                    command.CommandText = @"UPDATE [bbab].[dbo].[Weapon] SET
                        [MAKE] = @make,
                        [MODEL] = @model,
                        [CALIBER] = @caliber,
                        [SERIAL_NUMBER] = @serial,
                        [PRICE] = @price
                        WHERE [WEAPON_ID] = @weaponid";

                    //Add parameters to the statement
                    command.Parameters.Add("@make", SqlDbType.NVarChar, 50).Value = model._make;
                    command.Parameters.Add("@model", SqlDbType.NVarChar, 50).Value = model._model;
                    command.Parameters.Add("@caliber", SqlDbType.NVarChar, 10).Value = model._caliber;
                    command.Parameters.Add("@serial", SqlDbType.NVarChar, 25).Value = model._serialNumber;
                    command.Parameters.Add("price", SqlDbType.Float).Value = model._price;
                    command.Parameters.Add("@weaponid", SqlDbType.Int).Value = model._id;

                    //Prepare the statement
                    command.Prepare();

                    //Execute the query
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw new RecordNotUpdatedException("Weapon was not updated.", e.InnerException);
            }
        }
    }
}
    