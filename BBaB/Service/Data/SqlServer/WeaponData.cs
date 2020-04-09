using BBaB.Models;
using BBaB.Service.CrudExceptions.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using BBaB.Service.Data;
using BBaB.Utility.Interfaces;

namespace BBaB.Services.Data
{
    public class WeaponData : ICrud<WeaponModel>
    {
        private SqlConnection _connection;
        private IBBaBLogger logger;

        public WeaponData(SqlConnection connection, IBBaBLogger logger)
        {
            this._connection = connection;
            this.logger = logger;
        }

        /**
        * <see cref="ICrud{T}"/>
        */
        public void CreateT(WeaponModel model)
        {
            this.logger.Info("Entering WeaponData@CreateT");
            try
            {

                //Create connection and command
                this.logger.Info("Creating SqlCommand");
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Genereate sql script into command
                    this.logger.Info("Generating sql script");
                    command.CommandText = @"INSERT INTO [bbab].[dbo].[Weapon] 
                    ([MAKE], [MODEL], [CALIBER], [SERIAL_NUMBER], [PRICE]) 
                    VALUES (@make, @model, @caliber, @serial, @price)";

                    //Add parameters to the command string
                    this.logger.Info("Binding data to sql");
                    command.Parameters.Add("@make", SqlDbType.NVarChar, 50).Value = model._make;
                    command.Parameters.Add("@model", SqlDbType.NVarChar, 50).Value = model._model;
                    command.Parameters.Add("@caliber", SqlDbType.NVarChar, 10).Value = model._caliber;
                    command.Parameters.Add("@serial", SqlDbType.NVarChar, 25).Value = model._serialNumber;
                    command.Parameters.Add("@price", SqlDbType.Float).Value = model._price;

                    //Prepare the statement
                    this.logger.Info("Preparing command");
                    command.Prepare();

                    //Execute the command
                    this.logger.Info("Executing command NonQuery");
                    command.ExecuteNonQuery();
                }
                this.logger.Info("Exiting WeaponData@CreateT");
            }
            catch (Exception e)
            {
                this.logger.Error("Catching Exception", e);
                this.logger.Info("Throwing RecordNotCreatedException");
                throw new RecordNotCreatedException("Record was not created, place try again or contact support.", e.InnerException);
            }
        }

        /**
        * <see cref="ICrud{T}"/>
        */
        public void DeleteT(WeaponModel model)
        {
            this.logger.Info("Entering WeaponData@DeleteT");
            try
            {
                //Create the connection and command
                this.logger.Info("Creating SqlCommand");
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Generate sql script
                    this.logger.Info("Generating sql script");
                    command.CommandText = @"DELETE FROM [bbab].[dbo].[Weapon] where [WEAPON_ID] = @weaponid";

                    //Add parameters to command
                    this.logger.Info("Binding data to sql");
                    command.Parameters.Add("@weaponid", SqlDbType.Int).Value = model._id;

                    //Prepare the statement
                    this.logger.Info("Preparing command");
                    command.Prepare();

                    //Execute the statement
                    this.logger.Info("Executing command NonQuery");
                    command.ExecuteNonQuery();
                }

                this.logger.Info("Exiting WeaponData@DeleteT");
            }
            catch (Exception e)
            {
                this.logger.Error("Catching Exception", e);
                this.logger.Info("Throwing RecordNotDeleteException");
                throw new RecordNotDeletedException("Weapon data not deleted.", e.InnerException);
            }
        }

        /**
        * <see cref="ICrud{T}"/>
        */
        public List<WeaponModel> ReadAllT()
        {
            this.logger.Info("Entering WeaponData@ReadAllT");
            try
            {
                //Create temp model to store the data
                List<WeaponModel> weapons;

                //Create the connection and command
                this.logger.Info("Creating SqlCommand");
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //write the sql script to the command
                    this.logger.Info("Generating sql script");
                    command.CommandText = @"select [WEAPON_ID], [MAKE], [MODEL], [CALIBER], [SERIAL_NUMBER], [PRICE]
                                        from [bbab].[dbo].[Weapon]";

                    //prepare the statement
                    this.logger.Info("Preparing command");
                    command.Prepare();


                    //read the data recieved
                    this.logger.Info("Executing command ExecuteReader");
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //Don't read if no rows were returned
                        this.logger.Info("Checking if data exists");
                        if (reader.HasRows)
                        {
                            weapons = new List<WeaponModel>();
                            WeaponModel temp;
                            //Iterate through all the rows
                            this.logger.Info("Reading in data");
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

                                this.logger.Info("Pusing " + temp._serialNumber + " to the list");
                                weapons.Add(temp);

                            }
                        }
                        else
                        {
                            this.logger.Info("Closing the reader");
                            reader.Close();

                            this.logger.Info("Throwing RecordNotFoundException");
                            throw new RecordNotFoundException("No Weapons found to return.");
                        }

                        //Close the reader
                        this.logger.Info("Closing the reader");
                        reader.Close();
                    }
                }

                //return the model
                this.logger.Info("Returning list of weapons from WeaponData@ReadAllT");
                return weapons;

            }
            catch (Exception e)
            {
                this.logger.Error("Catching Exception", e);
                this.logger.Info("Throwing RecordNotFoundException");
                throw new RecordNotFoundException("No Weapons found to return", e.InnerException);
            }
        }

        /**
        * <see cref="ICrud{T}"/>
        */
        public List<WeaponModel> ReadBetweenT(int low, int high)
        {
            this.logger.Info("Entering WeaponDAta@ReadBetweenT");
            try
            {
                //Create temp model to store the data
                List<WeaponModel> weapons;

                //Create the connection and command
                this.logger.Info("Creating SqlCommand");
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //write the sql script to the command
                    this.logger.Info("Generating sql script");
                    command.CommandText = @"select [WEAPON_ID], [MAKE], [MODEL], [CALIBER], [SERIAL_NUMBER], [PRICE]
                                        from [bbab].[dbo].[Weapon]
                                        where [WEAPON_ID] BETWEEN @low AND @high";

                    //add in parameters to the sql script
                    this.logger.Info("Binding data to sql");
                    command.Parameters.Add("@low", SqlDbType.Int).Value = low;
                    command.Parameters.Add("@high", SqlDbType.Int).Value = high;

                    //prepare the statement
                    this.logger.Info("Preparing command");
                    command.Prepare();


                    //read the data recieved
                    this.logger.Info("Executing command ExecuteReader");
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //Don't read if no rows were returned
                        this.logger.Info("Checking if data exists");
                        if (reader.HasRows)
                        {
                            weapons = new List<WeaponModel>();
                            WeaponModel temp;
                            //Iterate through all the rows
                            this.logger.Info("Reading in data");
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

                                this.logger.Info("Pushing " + temp._serialNumber + " to the list");
                                weapons.Add(temp);

                            }
                        }
                        else
                        {
                            this.logger.Info("Closing the reader");
                            reader.Close();

                            this.logger.Info("Throwing RecordNotFoundException");
                            throw new RecordNotFoundException("No Weapons found to return.");
                        }

                        //Close the reader
                        this.logger.Info("Closing the reader");
                        reader.Close();
                    }
                }

                //return the model
                this.logger.Info("Returning list of weapons from WeaponData@ReadBetweenT");
                return weapons;

            }
            catch (Exception e)
            {
                this.logger.Error("Catching Exception", e);
                this.logger.Info("Throwing RecordNotFoundException");
                throw new RecordNotFoundException("No weapons found to return", e.InnerException);
            }
        }

        /**
         * <remarks>Search by any combination of make, model, caliber, serial</remarks>
        * <see cref="ICrud{T}"/>
        */
        public WeaponModel ReadTByField(WeaponModel model)
        {
            this.logger.Info("Entering ReadTByField");
            try
            {

                //Create the connection and command
                this.logger.Info("Creating SqlCommand");
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //write the sql script to the command
                    this.logger.Info("Generating sql script");
                    command.CommandText = @"select [WEAPON_ID], [MAKE], [MODEL], [CALIBER], [SERIAL_NUMBER], [PRICE]
                                        from [bbab].[dbo].[Weapon]
                                        where [MAKE] LIKE @make
										OR [MODEL] LIKE @model
										OR [CALIBER] LIKE @caliber
										OR [SERIAL_NUMBER] LIKE @serial";

                    //add in parameters to the sql script
                    this.logger.Info("Binding data to sql");
                    command.Parameters.Add("@make", SqlDbType.NVarChar, 50).Value = model._make;
                    command.Parameters.Add("@model", SqlDbType.NVarChar, 50).Value = model._model;
                    command.Parameters.Add("@caliber", SqlDbType.NVarChar, 10).Value = model._caliber;
                    command.Parameters.Add("@serial", SqlDbType.NVarChar, 25).Value = model._serialNumber;

                    //prepare the statement
                    this.logger.Info("Preparing command");
                    command.Prepare();

                    //read the data recieved'
                    this.logger.Info("Executing command ExecuteReader");
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //Don't read if no rows were returned
                        this.logger.Info("Checking if data exists");
                        if (reader.HasRows)
                        {
                            //Iterate through all the rows
                            this.logger.Info("Reading in data");
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
                            this.logger.Info("Closing the reader");
                            reader.Close();

                            this.logger.Info("Throwing RecordNotFoundException");
                            throw new RecordNotFoundException("No Weapons found to return.");
                        }

                        //Close the reader
                        this.logger.Info("Closing the reader");
                        reader.Close();
                    }
                }

                //return the model
                this.logger.Info("Returning weapon model from WeaponData@ReadTByField");
                return model;

            }
            catch (Exception e)
            {
                this.logger.Error("Catching Exception", e);
                this.logger.Info("Throwing RecordNotFoundException");
                throw new RecordNotFoundException("No weapon found to return.", e.InnerException);
            }
        }

        /**
        * <see cref="ICrud{T}"/>
        */
        public WeaponModel ReadTById(int id)
        {
            this.logger.Info("Entering ReadTById");
            try
            {
                WeaponModel temp = new WeaponModel();
                //Create the connection and command
                this.logger.Info("Creating SqlCommand");
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //write the sql script to the command
                    this.logger.Info("Generating sql script");
                    command.CommandText = @"select [WEAPON_ID], [MAKE], [MODEL], [CALIBER], [SERIAL_NUMBER], [PRICE]
                                        from [bbab].[dbo].[Weapon]
                                        where [WEAPON_ID] = @weaponid";

                    //add in parameters to the sql script
                    this.logger.Info("Binding data to sql");
                    command.Parameters.Add("@weaponid", SqlDbType.Int).Value = id;

                    //prepare the statement
                    this.logger.Info("Preparing command");
                    command.Prepare();

                    //read the data recieved
                    this.logger.Info("Executing command ExecuteReader");
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //Don't read if no rows were returned
                        this.logger.Info("Checking if data exists");
                        if (reader.HasRows)
                        {
                            //Iterate through all the rows
                            this.logger.Info("Reading in data");
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
                            this.logger.Info("Closing the reader");
                            reader.Close();

                            this.logger.Info("Throwing RecordNotFoundException");
                            throw new RecordNotFoundException("No Weapons found to return.");
                        }

                        //Close the reader
                        this.logger.Info("Closing the reader");
                        reader.Close();
                    }
                }

                //return the model
                this.logger.Info("Returning the weapon model from WeaponData@ReadTById");
                return temp;

            }
            catch (Exception e)
            {
                this.logger.Error("Catching Exception", e);
                this.logger.Info("Throwing RecordNotFoundException");
                throw new RecordNotFoundException("User not found", e.InnerException);
            }
        }

        /**
        * <see cref="ICrud{T}"/>
        */
        public void UpdateT(WeaponModel model)
        {
            this.logger.Info("Entering WeaponData@UpdateT");
            try
            {
                //Create the connection and command
                this.logger.Info("Creating SqlCommand");
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Generate the command sql statement
                    this.logger.Info("Generating sql script");
                    command.CommandText = @"UPDATE [bbab].[dbo].[Weapon] SET
                        [MAKE] = @make,
                        [MODEL] = @model,
                        [CALIBER] = @caliber,
                        [SERIAL_NUMBER] = @serial,
                        [PRICE] = @price
                        WHERE [WEAPON_ID] = @weaponid";

                    //Add parameters to the statement
                    this.logger.Info("Binding data to sql");
                    command.Parameters.Add("@make", SqlDbType.NVarChar, 50).Value = model._make;
                    command.Parameters.Add("@model", SqlDbType.NVarChar, 50).Value = model._model;
                    command.Parameters.Add("@caliber", SqlDbType.NVarChar, 10).Value = model._caliber;
                    command.Parameters.Add("@serial", SqlDbType.NVarChar, 25).Value = model._serialNumber;
                    command.Parameters.Add("price", SqlDbType.Float).Value = model._price;
                    command.Parameters.Add("@weaponid", SqlDbType.Int).Value = model._id;

                    //Prepare the statement
                    this.logger.Info("Preparing command");
                    command.Prepare();

                    //Execute the query
                    this.logger.Info("Executing command NonQuery");
                    command.ExecuteNonQuery();
                }
                this.logger.Info("Exiting WeaponData@UpdateT");
            }
            catch (Exception e)
            {
                this.logger.Error("Catching Exception", e);
                this.logger.Info("Throwing RecordNotUpdateException");
                throw new RecordNotUpdatedException("Weapon was not updated.", e.InnerException);
            }
        }
    }
}
    