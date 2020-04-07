using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using BBaB.Models;
using BBaB.Service.CrudExceptions.Exceptions;
using BBaB.Service.Data;

namespace BBaB.Services.Data
{
    public class AddressData : ICrud<AddressModel>
    {
        private SqlConnection _connection;

        public AddressData(SqlConnection connection)
        {
            this._connection = connection;
        }

        public void CreateT(AddressModel model)
        {
            try
            {
                //Create connection and command
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Generate sql script into command
                    command.CommandText = @"INSERT INTO [bbab].[dbo].[Address]
                                        ([ADDRESS_1], [ADDRESS_2], [ADDRESS_3], [CITY], [STATE], [COUNTRY], [ZIP] [PRINCIPAL_ID])
                                        VALUES (@address1, @address2, @address3, @city, @state, @country, @zip, @principal";

                    //Bind the parameters
                    command.Parameters.Add("@address1", SqlDbType.NVarChar, 100).Value = model._address1;
                    command.Parameters.Add("@address2", SqlDbType.NVarChar, 50).Value = model._address2;
                    command.Parameters.Add("@address3", SqlDbType.NVarChar, 50).Value = model._address3;
                    command.Parameters.Add("@city", SqlDbType.NVarChar, 30).Value = model._city;
                    command.Parameters.Add("@state", SqlDbType.NVarChar, 50).Value = model._state;
                    command.Parameters.Add("@country", SqlDbType.NVarChar, 60).Value = model._country;
                    command.Parameters.Add("@zip", SqlDbType.NVarChar, 5).Value = model._zip;
                    command.Parameters.Add("@principal", SqlDbType.Int).Value = model._resident._id;

                    //Prepare the statement
                    command.Prepare();

                    //Execute the statement
                    command.ExecuteNonQuery();
                }
            }catch(Exception e)
            {
                throw new RecordNotCreatedException("Address was not created", e.InnerException);
            }
        }

        public void DeleteT(AddressModel model)
        {
            try
            {
                //Create the connection and command
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Generate the sql
                    command.CommandText = @"DELETE FROM [bbab].[dbo].[Address] WHERE [ADDRESS_ID] = @addressid";

                    //Bind parameters
                    command.Parameters.Add("@addressid", SqlDbType.Int).Value = model._id;

                    //Prepare the statement
                    command.Prepare();

                    //Execute the statement
                    command.ExecuteNonQuery();
                }
            }catch(Exception e)
            {
                throw new RecordNotDeletedException("Address was not deleted", e.InnerException);
            }
        }

        public List<AddressModel> ReadAllT()
        {
            try
            {
                List<AddressModel> addresses;
                //Create the connection and command
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Generate the sql
                    command.CommandText = @"SELECT [ADDRESS_ID], [ADDRESS_1], [ADDRESS_2], [ADDRESS_3], [CITY], [STATE], [COUNTRY], [ZIP], 
                                            [FULL_NAME], [EMAIL], [PHONE_NUMBER]
                                            FROM [bbab].[dbo].[Address] as a 
                                            LEFT JOIN [bbab].[dbo].[Principal] as p on a.[PRINCIPAL_ID] = p.[PRINCIPAL_ID]";

                    //Prepare the statement
                    command.Prepare();

                    //Execute the statement
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if(reader.HasRows)
                        {
                            addresses = new List<AddressModel>();
                            AddressModel temp;
                            while(reader.Read())
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

                                addresses.Add(temp);
                            }
                        }
                        else
                        {
                            reader.Close();
                            throw new RecordNotFoundException();
                        }

                        //close the reader
                        reader.Close();
                    }
                  

                 //return the list
                return addresses;
                }
            }catch(Exception e)
            {
                throw new RecordNotFoundException("No addresses were found in the database", e.InnerException);
            }
        }

        public List<AddressModel> ReadBetweenT(int low, int high)
        {
            try
            {
                List<AddressModel> addresses;
                //Create the connection and command
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Generate the sql
                    command.CommandText = @"SELECT [ADDRESS_ID], [ADDRESS_1], [ADDRESS_2], [ADDRESS_3], [CITY], [STATE], [COUNTRY], [ZIP], 
                                            [FULL_NAME], [EMAIL], [PHONE_NUMBER]
                                            FROM [bbab].[dbo].[Address] as a 
                                            LEFT JOIN [bbab].[dbo].[Principal] as p on a.[PRINCIPAL_ID] = p.[PRINCIPAL_ID]
                                            AND [ADDRESS_ID] BETWEEN @low AND @high";

                    //Bind paramters
                    command.Parameters.Add("@low", SqlDbType.Int).Value = low;
                    command.Parameters.Add("@high", SqlDbType.Int).Value = high;

                    //Prepare the statement
                    command.Prepare();

                    //Execute the statement
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            addresses = new List<AddressModel>();
                            AddressModel temp;
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

                                addresses.Add(temp);
                            }
                        }
                        else
                        {
                            reader.Close();
                            throw new RecordNotFoundException();
                        }

                        //close the reader
                        reader.Close();
                    }

                    //return the list
                    return addresses;
                }
            }
            catch (Exception e)
            {
                throw new RecordNotFoundException("No addresses were found in the database", e.InnerException);
            }
        }

        /**
        * <remarks>Search for addresse by a user firstname</remarks>
        */
        public AddressModel ReadTByField(AddressModel model)
        {
            try
            {
                AddressModel address;
                //Create the connection and command
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Generate the sql
                    command.CommandText = @"SELECT [ADDRESS_ID], [ADDRESS_1], [ADDRESS_2], [ADDRESS_3], [CITY], [STATE], [COUNTRY], [ZIP], 
                                            [FULL_NAME], [EMAIL], [PHONE_NUMBER]
                                            FROM [bbab].[dbo].[Address] as a 
                                            LEFT JOIN [bbab].[dbo].[Principal] as p on a.[PRINCIPAL_ID] = p.[PRINCIPAL_ID]
                                            WHERE p.[EMAIL] = @email";

                    //Bind paramters
                    command.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = model._resident._credentials._email;

                    //Prepare the statement
                    command.Prepare();

                    //Execute the statement
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            address = new AddressModel();
                            while (reader.Read())
                            {
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
                        }
                        else
                        {
                            reader.Close();
                            throw new RecordNotFoundException();
                        }

                        //close the reader
                        reader.Close();
                    }

                    //return the list
                    return address;
                }
            }
            catch (Exception e)
            {
                throw new RecordNotFoundException("No addresses were found in the database", e.InnerException);
            }
        }

        public AddressModel ReadTById(int id)
        {
            try
            {
                AddressModel address;
                //Create the connection and command
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Generate the sql
                    command.CommandText = @"SELECT [ADDRESS_ID], [ADDRESS_1], [ADDRESS_2], [ADDRESS_3], [CITY], [STATE], [COUNTRY], [ZIP], 
                                            [FULL_NAME], [EMAIL], [PHONE_NUMBER]
                                            FROM [bbab].[dbo].[address] as a 
                                            LEFT JOIN [bbab].[dbo].[Principal] as p on a.[PRINCIPAL_ID] = p.[PRINCIPAL_ID]
                                            WHERE a.[ADDRESS_ID] = @addressid";

                    //Bind paramters
                    command.Parameters.Add("@addressid", SqlDbType.Int).Value = id;

                    //Prepare the statement
                    command.Prepare();

                    //Execute the statement
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            address = new AddressModel();
                            while (reader.Read())
                            {
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
                        }
                        else
                        {
                            reader.Close();
                            throw new RecordNotFoundException();
                        }

                        //close the reader
                        reader.Close();
                    }

                    //return the list
                    return address;
                }
            }
            catch (Exception e)
            {
                throw new RecordNotFoundException("No addresses were found in the database", e.InnerException);
            }
        }

        public void UpdateT(AddressModel model)
        {
            try
            {
                //Create the connection and command
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Generate the command sql statement
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
                    command.Parameters.Add("@address1", SqlDbType.NVarChar, 50).Value = model._address1;
                    command.Parameters.Add("@address2", SqlDbType.NVarChar, 100).Value = model._address2;
                    command.Parameters.Add("@address3", SqlDbType.NVarChar, 20).Value = model._address3;
                    command.Parameters.Add("@city", SqlDbType.Bit).Value = model._city;
                    command.Parameters.Add("@state", SqlDbType.NVarChar, 10).Value = model._state;
                    command.Parameters.Add("@zip", SqlDbType.NVarChar, 64).Value = model._zip;
                    command.Parameters.Add("@addressid", SqlDbType.Int).Value = model._id;

                    //Prepare the statement
                    command.Prepare();

                    //Execute the query
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw new RecordNotUpdatedException("Address was not updated.", e.InnerException);
            }
        }

    }
}