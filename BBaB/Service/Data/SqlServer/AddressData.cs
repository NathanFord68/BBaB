using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using BBaB.Models;
using BBaB.Service.CrudExceptions.Exceptions;
using BBaB.Service.Data;
using BBaB.Utility.Interfaces;

namespace BBaB.Services.Data
{
    public class AddressData : ICrud<AddressModel>
    {
        private SqlConnection _connection;
        private IBBaBLogger logger;

        public AddressData(SqlConnection connection, IBBaBLogger logger)
        {
            this._connection = connection;
            this.logger = logger;
        }

        public void CreateT(AddressModel model)
        {
            this.logger.Info("Entering AddressData@CreateT");
            try
            {
                //Create connection and command
                this.logger.Info("Creating SqlCommand");
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Generate sql script into command
                    this.logger.Info("Generating sql script");
                    command.CommandText = @"INSERT INTO [bbab].[dbo].[Address]
                                        ([ADDRESS_1], [ADDRESS_2], [ADDRESS_3], [CITY], [STATE], [COUNTRY], [ZIP] [PRINCIPAL_ID])
                                        VALUES (@address1, @address2, @address3, @city, @state, @country, @zip, @principal";

                    //Bind the parameters
                    this.logger.Info("Binding data to sql");
                    command.Parameters.Add("@address1", SqlDbType.NVarChar, 100).Value = model._address1;
                    command.Parameters.Add("@address2", SqlDbType.NVarChar, 50).Value = model._address2;
                    command.Parameters.Add("@address3", SqlDbType.NVarChar, 50).Value = model._address3;
                    command.Parameters.Add("@city", SqlDbType.NVarChar, 30).Value = model._city;
                    command.Parameters.Add("@state", SqlDbType.NVarChar, 50).Value = model._state;
                    command.Parameters.Add("@country", SqlDbType.NVarChar, 60).Value = model._country;
                    command.Parameters.Add("@zip", SqlDbType.NVarChar, 5).Value = model._zip;
                    command.Parameters.Add("@principal", SqlDbType.Int).Value = model._resident._id;

                    //Prepare the statement
                    this.logger.Info("Preparing command");
                    command.Prepare();

                    //Execute the statement
                    this.logger.Info("Executing command NonQuery");
                    command.ExecuteNonQuery();

                    this.logger.Info("Exiting AddressData@CreateT");
                }
            }catch(Exception e)
            {
                this.logger.Error("Catching Exception", e);
                this.logger.Info("Throwing RecordNotCreatedException");
                throw new RecordNotCreatedException("Address was not created", e.InnerException);
            }
        }

        public void DeleteT(AddressModel model)
        {
            this.logger.Info("Entering AddressData@DeleteT");
            try
            {
                //Create the connection and command
                this.logger.Info("Creating SqlCommand");
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Generate the sql
                    this.logger.Info("Generating Sql Script");
                    command.CommandText = @"DELETE FROM [bbab].[dbo].[Address] WHERE [ADDRESS_ID] = @addressid";

                    //Bind parameters
                    this.logger.Info("Binding data to sql");
                    command.Parameters.Add("@addressid", SqlDbType.Int).Value = model._id;

                    //Prepare the statement
                    this.logger.Info("Preparing the command");
                    command.Prepare();

                    //Execute the statement
                    this.logger.Info("Executing command NonQuery");
                    command.ExecuteNonQuery();

                    this.logger.Info("Exiting AddressData@DeleteT");
                }
            }catch(Exception e)
            {
                this.logger.Error("Catching Exception", e);
                this.logger.Info("Throwing RecordNotDeletedException");
                throw new RecordNotDeletedException("Address was not deleted", e.InnerException);
            }
        }

        public List<AddressModel> ReadAllT()
        {
            this.logger.Info("Entering AddressData@ReadAddT");
            try
            {
                List<AddressModel> addresses;
                //Create the connection and command
                this.logger.Info("Creating SqlCommand");
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Generate the sql
                    this.logger.Info("Generating sql script");
                    command.CommandText = @"SELECT [ADDRESS_ID], [ADDRESS_1], [ADDRESS_2], [ADDRESS_3], [CITY], [STATE], [COUNTRY], [ZIP], 
                                            [FULL_NAME], [EMAIL], [PHONE_NUMBER]
                                            FROM [bbab].[dbo].[Address] as a 
                                            LEFT JOIN [bbab].[dbo].[Principal] as p on a.[PRINCIPAL_ID] = p.[PRINCIPAL_ID]";

                    //Prepare the statement
                    this.logger.Info("Preparing command");
                    command.Prepare();

                    //Execute the statement
                    this.logger.Info("Executing command ExecuteReader");
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        this.logger.Info("Checking if data exists");
                        if (reader.HasRows)
                        {
                            addresses = new List<AddressModel>();
                            AddressModel temp;
                            this.logger.Info("Reading in data");
                            while (reader.Read())
                            {
                                temp = new AddressModel();
                                temp._id = reader.GetInt32(0);
                                temp._address1 = reader.GetString(1);
                                temp._address2 = reader.GetString(2);
                                temp._address3 = reader.GetString(3);
                                temp._city = reader.GetString(4);
                                temp._state = reader.GetString(5);
                                temp._country = reader.GetString(6);
                                temp._zip = reader.GetString(7);
                                temp._resident._fullName = reader.GetString(8);
                                temp._resident._credentials._email = reader.GetString(9);
                                temp._resident._phoneNumber = reader.GetString(10);

                                this.logger.Info("Pushing " + temp._resident._fullName + "'s address to list");
                                addresses.Add(temp);
                            }
                        }
                        else
                        {
                            this.logger.Info("Closing reader");
                            reader.Close();

                            this.logger.Info("Throwing RecordNotFoundException");
                            throw new RecordNotFoundException();
                        }

                        //close the reader
                        this.logger.Info("Closing reader");
                        reader.Close();
                    }


                    //return the list
                    this.logger.Info("Returning list of addresses from AddressData@ReadAllT");
                    return addresses;
                }
            }catch(Exception e)
            {
                this.logger.Error("Catching Exception", e);
                this.logger.Info("Throwing RecordNotFoundException");
                throw new RecordNotFoundException("No addresses were found in the database", e.InnerException);
            }
        }

        public List<AddressModel> ReadBetweenT(int low, int high)
        {
            this.logger.Info("Entering AddressData@ReadBetweenT");
            try
            {
                List<AddressModel> addresses;
                //Create the connection and command
                this.logger.Info("Creating SqlCommand");
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Generate the sql
                    this.logger.Info("Generating Sql script");
                    command.CommandText = @"SELECT [ADDRESS_ID], [ADDRESS_1], [ADDRESS_2], [ADDRESS_3], [CITY], [STATE], [COUNTRY], [ZIP], 
                                            [FULL_NAME], [EMAIL], [PHONE_NUMBER]
                                            FROM [bbab].[dbo].[Address] as a 
                                            LEFT JOIN [bbab].[dbo].[Principal] as p on a.[PRINCIPAL_ID] = p.[PRINCIPAL_ID]
                                            AND [ADDRESS_ID] BETWEEN @low AND @high";

                    //Bind paramters
                    this.logger.Info("Binding data to sql");
                    command.Parameters.Add("@low", SqlDbType.Int).Value = low;
                    command.Parameters.Add("@high", SqlDbType.Int).Value = high;

                    //Prepare the statement
                    this.logger.Info("Preparing command");
                    command.Prepare();

                    //Execute the statement
                    this.logger.Info("Executing command ExecuteReader");
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        this.logger.Info("Checking if data exists");
                        if (reader.HasRows)
                        {
                            addresses = new List<AddressModel>();
                            AddressModel temp;
                            this.logger.Info("Reading in data");
                            while (reader.Read())
                            {
                                temp = new AddressModel();
                                temp._id = reader.GetInt32(0);
                                temp._address1 = reader.GetString(1);
                                temp._address2 = reader.GetString(2);
                                temp._address3 = reader.GetString(3);
                                temp._city = reader.GetString(4);
                                temp._state = reader.GetString(5);
                                temp._country = reader.GetString(6);
                                temp._zip = reader.GetString(7);
                                temp._resident._fullName = reader.GetString(8);
                                temp._resident._credentials._email = reader.GetString(9);
                                temp._resident._phoneNumber = reader.GetString(10);

                                this.logger.Info("Pushing " + temp._resident._fullName + "'s to the list");
                                addresses.Add(temp);
                            }
                        }
                        else
                        {
                            this.logger.Info("Closing the reader");
                            reader.Close();

                            this.logger.Info("Throwing record not found Exception");
                            throw new RecordNotFoundException();
                        }

                        //close the reader
                        this.logger.Info("Closing the reader");
                        reader.Close();
                    }

                    //return the list
                    this.logger.Info("Returning list of address from AddressData@ReadBetweenT");
                    return addresses;
                }
            }
            catch (Exception e)
            {
                this.logger.Error("Catching Exception", e);
                this.logger.Info("Throwing RecordNotFoundException");
                throw new RecordNotFoundException("No addresses were found in the database", e.InnerException);
            }
        }

        /**
        * <remarks>Search for addresse by a user firstname</remarks>
        */
        public AddressModel ReadTByField(AddressModel model)
        {
            this.logger.Info("Entering AddressData@ReadTByField");
            try
            {
                AddressModel address;
                //Create the connection and command
                this.logger.Info("Creating SqlCommand");
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Generate the sql
                    this.logger.Info("Generating sql script");
                    command.CommandText = @"SELECT [ADDRESS_ID], [ADDRESS_1], [ADDRESS_2], [ADDRESS_3], [CITY], [STATE], [COUNTRY], [ZIP], 
                                            [FULL_NAME], [EMAIL], [PHONE_NUMBER]
                                            FROM [bbab].[dbo].[Address] as a 
                                            LEFT JOIN [bbab].[dbo].[Principal] as p on a.[PRINCIPAL_ID] = p.[PRINCIPAL_ID]
                                            WHERE p.[EMAIL] = @email";

                    //Bind paramters
                    this.logger.Info("Binding data to sql");
                    command.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = model._resident._credentials._email;

                    //Prepare the statement
                    this.logger.Info("Preparing command");
                    command.Prepare();

                    //Execute the statement
                    this.logger.Info("Executing command ExecuteReader");
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        this.logger.Info("Checking if data exists");
                        if (reader.HasRows)
                        {
                            address = new AddressModel();
                            this.logger.Info("Reading in data");
                            reader.Read();
                            address._id = reader.GetInt32(0);
                            address._address1 = reader.GetString(1);
                            address._address2 = reader.GetString(2);
                            address._address3 = reader.GetString(3);
                            address._city = reader.GetString(4);
                            address._state = reader.GetString(5);
                            address._country = reader.GetString(6);
                            address._zip = reader.GetString(7);
                            address._resident._fullName = reader.GetString(8);
                            address._resident._credentials._email = reader.GetString(9);
                            address._resident._phoneNumber = reader.GetString(10);
                        }
                        else
                        {
                            this.logger.Info("Closing reader");
                            reader.Close();

                            this.logger.Info("Throwing RecordNotFoundException");
                            throw new RecordNotFoundException();
                        }

                        //close the reader
                        this.logger.Info("Closing reader");
                        reader.Close();
                    }

                    //return the list
                    this.logger.Info("Return address from AddressData@ReadTByField");
                    return address;
                }
            }
            catch (Exception e)
            {
                this.logger.Error("Catching Exception", e);
                this.logger.Info("Throwing RecordNotFoundException");
                throw new RecordNotFoundException("No addresses were found in the database", e.InnerException);
            }
        }

        public AddressModel ReadTById(int id)
        {
            this.logger.Info("Entering AddressData@ReadTById");
            try
            {
                
                AddressModel address;
                //Create the connection and command
                this.logger.Info("Creating SqlCommand");
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Generate the sql
                    this.logger.Info("Generating sql script");
                    command.CommandText = @"SELECT [ADDRESS_ID], [ADDRESS_1], [ADDRESS_2], [ADDRESS_3], [CITY], [STATE], [COUNTRY], [ZIP], 
                                            [FULL_NAME], [EMAIL], [PHONE_NUMBER]
                                            FROM [bbab].[dbo].[address] as a 
                                            LEFT JOIN [bbab].[dbo].[Principal] as p on a.[PRINCIPAL_ID] = p.[PRINCIPAL_ID]
                                            WHERE a.[ADDRESS_ID] = @addressid";

                    //Bind paramters
                    this.logger.Info("Binding data to sql");
                    command.Parameters.Add("@addressid", SqlDbType.Int).Value = id;

                    //Prepare the statement
                    this.logger.Info("Preparing command");
                    command.Prepare();

                    //Execute the statement
                    this.logger.Info("Executing command ExecuteReader");
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        this.logger.Info("Checking if data exists");
                        if (reader.HasRows)
                        {
                            address = new AddressModel();
                            this.logger.Info("Reading in data");
                            reader.Read();
                            address._id = reader.GetInt32(0);
                            address._address1 = reader.GetString(1);
                            address._address2 = reader.GetString(2);
                            address._address3 = reader.GetString(3);
                            address._city = reader.GetString(4);
                            address._state = reader.GetString(5);
                            address._country = reader.GetString(6);
                            address._zip = reader.GetString(7);
                            address._resident._fullName = reader.GetString(8);
                            address._resident._credentials._email = reader.GetString(9);
                            address._resident._phoneNumber = reader.GetString(10);
                        }
                        else
                        {
                            this.logger.Info("Closing reader");
                            reader.Close();

                            this.logger.Info("Throwing RecordNotFoundException");
                            throw new RecordNotFoundException();
                        }

                        //close the reader
                        this.logger.Info("Closing reader");
                        reader.Close();
                    }

                    //return the list
                    this.logger.Info("Returning address from AddressData@ReadTById");
                    return address;
                }
            }
            catch (Exception e)
            {
                this.logger.Error("Catching Exception", e);
                this.logger.Info("Throwing RecordNotFoundException");
                throw new RecordNotFoundException("No addresses were found in the database", e.InnerException);
            }
        }

        public void UpdateT(AddressModel model)
        {
            this.logger.Info("Entering AddressData@UpdateT");
            try
            {
                //Create the connection and command
                this.logger.Info("Creating SqlCommand");
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Generate the command sql statement
                    this.logger.Info("Generating sql script");
                    command.CommandText = @"UPDATE [bbab].[dbo].[Address] SET
                        [ADDRESS_1] = @address1,
                        [ADDRESS_2] = @address2,
                        [ADDRESS_3] = @address3,
                        [CITY] = @city,
                        [STATE] = @state,
                        [COUNTRY] = @country,
                        [ZIP] = @zip
                        WHERE [ADDRESS_ID] = @addressid";

                    //Add parameters to the statement
                    this.logger.Info("Binding data to sql");
                    command.Parameters.Add("@address1", SqlDbType.NVarChar, 50).Value = model._address1;
                    command.Parameters.Add("@address2", SqlDbType.NVarChar, 100).Value = model._address2;
                    command.Parameters.Add("@address3", SqlDbType.NVarChar, 20).Value = model._address3;
                    command.Parameters.Add("@city", SqlDbType.Bit).Value = model._city;
                    command.Parameters.Add("@state", SqlDbType.NVarChar, 10).Value = model._state;
                    command.Parameters.Add("@zip", SqlDbType.NVarChar, 64).Value = model._zip;
                    command.Parameters.Add("@addressid", SqlDbType.Int).Value = model._id;

                    //Prepare the statement
                    this.logger.Info("Preparing command");
                    command.Prepare();

                    //Execute the query
                    this.logger.Info("Executing command NonQuery");
                    command.ExecuteNonQuery();
                }

                this.logger.Info("Exiting AddressData@UpdateT");
            }
            catch (Exception e)
            {
                this.logger.Error("Catching Exception", e);
                this.logger.Info("Throwing RecordNotUpdatedException");
                throw new RecordNotUpdatedException("Address was not updated.", e.InnerException);
            }
        }

    }
}