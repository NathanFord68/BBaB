using BBaB.Models;
using BBaB.Service.Data;
using BBaB.Service.CrudExceptions.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using BBaB.Utility.Interfaces;

namespace BBaB.Services.Data
{
    /**
     * <typeparam name="T"></typeparam>
     */
    public class AccountData : ICrud<PrincipalModel>
    {
        private SqlConnection _connection;
        private IBBaBLogger logger;

        public AccountData(SqlConnection connection, IBBaBLogger logger)
        {
            this._connection = connection;
            this.logger = logger;
        }

        /**
         * <inheritdoc/>
         * <see cref="ICrud{T}"/>
         */
        public void CreateT(PrincipalModel model)
        {
            this.logger.Info("Entering AccountData@CreateT");
            try
            {

                //Create command
                this.logger.Info("Creating SqlCommand");
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Genereate sql script into command
                    this.logger.Info("Generating Sql Script");
                    command.CommandText = @"INSERT INTO [bbab].[dbo].[Principal] 
                    ([FULL_NAME], [USER_NAME], [EMAIL], [PHONE_NUMBER], [SALT], [PASSWORD]) 
                    VALUES (@fullname, @username, @email, @phonenumber, @salt, @password)";

                    //Add parameters to the command string
                    this.logger.Info("Binding data to sql");
                    command.Parameters.Add("@fullname", SqlDbType.NVarChar, 50).Value = model._fullName;
                    command.Parameters.Add("@username", SqlDbType.NVarChar, 20).Value = model._userName;
                    command.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = model._credentials._email;
                    command.Parameters.Add("@phonenumber", SqlDbType.NVarChar, 10).Value = model._phoneNumber;
                    command.Parameters.Add("@salt", SqlDbType.NVarChar, 64).Value = model._salt;
                    command.Parameters.Add("@password", SqlDbType.NVarChar, 64).Value = model._credentials._password;

                    //Prepare the statement
                    this.logger.Info("Preparing the command");
                    command.Prepare();

                    //Execute the command
                    this.logger.Info("Executing command NonQuery");
                    command.ExecuteNonQuery();
                }

                this.logger.Info("Exiting AccountData@CreateT");
            }
            catch(Exception e)
            {
                this.logger.Error("Catching Exception", e);

                this.logger.Info("Throwing RecordNotCreatedException");
                throw new RecordNotCreatedException("Account was not created. Please try again or contact support.", e.InnerException);
            }
        }

        /**
         * <inheritdoc/>
         * <see cref="ICrud{T}"/>
         */
        public void DeleteT(PrincipalModel model)
        {
            this.logger.Info("Entering AccountData@DeleteT");
            try
            {
                //Create the connection and command
                this.logger.Info("Creating SqlCommand");
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Generate sql script
                    this.logger.Info("Generating Sql Script");
                    command.CommandText = @"DELETE FROM [bbab].[dbo].[Principal] where [PRINCIPAL_ID] = @principalid";

                    //Add parameters to command
                    this.logger.Info("Binding data to sql");
                    command.Parameters.Add("@principalid", SqlDbType.Int).Value = model._id;

                    //Prepare the statement
                    this.logger.Info("Preparing the command");
                    command.Prepare();

                    //Execute the statement
                    this.logger.Info("Executing command NonQuery");
                    command.ExecuteNonQuery();

                    this.logger.Info("Exiting AccountData@DeleteT");
                }
            }
            catch (Exception e)
            {
                this.logger.Error("Catching Exception", e);
                this.logger.Info("Throwing Exception RecordNotDeleteException");
                throw new RecordNotDeletedException("Account not deleted.", e.InnerException);
            }
        }

        /**
         * <inheritdoc/>
         * <see cref="ICrud{T}"/>
         */
        public List<PrincipalModel> ReadBetweenT(int low, int high)
        {
            this.logger.Info("Entering AccountData@ReadBetweenT");
            try
            {
                //Create temp model to store the data
                List<PrincipalModel> principals = new List<PrincipalModel>();

                //Create the connection and command
                this.logger.Info("Creating SqlCommand");
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //write the sql script to the command
                    this.logger.Info("Generating Sql Script");
                    command.CommandText = @"select p.[PRINCIPAL_ID], [FULL_NAME], [USER_NAME], [EMAIL], [EMAIL_CONFIRMED], [PHONE_NUMBER], coalesce([LEVEL], 0) as [LEVEL], COALESCE([TITLE], 'User') as [TITLE]
                                        from [bbab].[dbo].[Principal] AS p
										left join [bbab].[dbo].[Admin] as a 
										on p.[PRINCIPAL_ID] = a.[PRINCIPAL_ID]
                                        where p.[PRINCIPAL_ID] BETWEEN @low AND @high";

                    //add in parameters to the sql script
                    this.logger.Info("Binding data to sql");
                    command.Parameters.Add("@low", SqlDbType.Int).Value = low;
                    command.Parameters.Add("@high", SqlDbType.Int).Value = high;

                    //prepare the statement
                    this.logger.Info("Preparing the command");
                    command.Prepare();


                    //read the data recieved
                    this.logger.Info("Executing command ExecuteReader");
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //Don't read if no rows were returned
                        this.logger.Info("Checking if reader has data");
                        if (reader.HasRows)
                        {
                            PrincipalModel temp;
                            //Iterate through all the rows
                            this.logger.Info("Reading in the data");
                            while (reader.Read())
                            {
                                //Populate model and push onto List
                                temp = new PrincipalModel();
                                temp._id = reader.GetInt32(0);
                                temp._fullName = reader.GetString(1);
                                temp._userName = reader.GetString(2);
                                temp._credentials._email = reader.GetString(3);
                                temp._accountConfirmed = reader.GetByte(4) > 0;
                                temp._phoneNumber = reader.GetString(5);
                                temp._adminLevel = reader.GetInt32(6);
                                temp._adminTitle = reader.GetString(7);

                                this.logger.Info("Adding " + temp._fullName + " to the List");
                                principals.Add(temp);

                            }
                        }
                        else
                        {
                            this.logger.Info("Closing the reader due to no data present");
                            reader.Close();

                            this.logger.Info("Throwing RecordNotFoundException");
                            throw new RecordNotFoundException("No users found to return");
                        }

                        //Close the reader
                        this.logger.Info("Closing the reader");
                        reader.Close();
                    }
                }

                //return the model
                this.logger.Info("Returning list of users from AccountData@ReadBetweenT");
                return principals;

            }
            catch (Exception e)
            {
                this.logger.Error("Catching exception", e);
                this.logger.Info("Throwing RecordNotFoundException");
                throw new RecordNotFoundException("No Users found to return", e.InnerException);
            }
        }

        /**
         * <inheritdoc/>
         * <see cref="ICrud{T}"/>
         */
         public List<PrincipalModel> ReadAllT()
        {
            this.logger.Info("Entering AccountData@ReadAllT");
            try
            {
                //Create temp model to store the data
                List<PrincipalModel> principals = new List<PrincipalModel>();

                //Create the connection and command
                this.logger.Info("Creating SqlCommand");
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //write the sql script to the command
                    this.logger.Info("Generating Sql Script");
                    command.CommandText = @"select p.[PRINCIPAL_ID], [FULL_NAME], [USER_NAME], [EMAIL], [EMAIL_CONFIRMED], [PHONE_NUMBER], coalesce([LEVEL], 0) as [LEVEL], COALESCE([TITLE], 'User') as [TITLE]
                                        from [bbab].[dbo].[Principal] as p
                                        left join [bbab].[dbo].[Admin] as a 
                                        ON p.[PRINCIPAL_ID] = a.[PRINCIPAL_ID]";

                    //prepare the statement
                    this.logger.Info("Preparing the command");
                    command.Prepare();


                    //read the data recieved
                    this.logger.Info("Executing command ExecuteReader");
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        //Don't read if no rows were returned
                        this.logger.Info("Checking if data exists");
                        if (reader.HasRows)
                        {
                            PrincipalModel temp;

                            //Iterate through all the rows
                            this.logger.Info("Read in the data");
                            while (reader.Read())
                            {
                                //Populate model and push onto List
                                temp = new PrincipalModel();
                                temp._id = reader.GetInt32(0);
                                temp._fullName = reader.GetString(1);
                                temp._userName = reader.GetString(2);
                                temp._credentials._email = reader.GetString(3);
                                temp._accountConfirmed = reader.GetByte(4) > 0;
                                temp._phoneNumber = reader.GetString(5);
                                temp._adminLevel = reader.GetInt32(6);
                                temp._adminTitle = reader.GetString(7);

                                this.logger.Info("Adding " + temp._fullName + " to list");
                                principals.Add(temp);

                            }
                        }
                        else
                        {
                            this.logger.Info("Closing the reader");
                            reader.Close();

                            this.logger.Info("Throwing RecordNotFoundException");
                            throw new RecordNotFoundException("No users found to return");
                        }

                        //Close the reader
                        this.logger.Info("Closing the reader");
                        reader.Close();
                    }
                }

                //return the model
                this.logger.Info("Returing list of users from AccountData@ReadAllT");
                return principals;

            }
            catch (Exception e)
            {
                this.logger.Error("Catching Exception", e);
                this.logger.Info("Throwing RecordNotFoundException");
                throw new RecordNotFoundException("No Users found to return", e.InnerException);
            }
        }

        /**
         * <inheritdoc/>
         * <see cref="ICrud{T}"/>
         */
        public PrincipalModel ReadTById(int id)
        {
            this.logger.Info("Entering AccountData@ReadTById");
            try
            {
                //Create temp model to store the data
                PrincipalModel temp;

                //Create the connection and command
                this.logger.Info("Creating SqlCommand");
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //write the sql script to the command
                    this.logger.Info("Generating Sql Script");
                    command.CommandText = @"select p.[PRINCIPAL_ID], [FULL_NAME], [USER_NAME], [EMAIL], [EMAIL_CONFIRMED], [PHONE_NUMBER], coalesce([LEVEL], 0) as [LEVEL], COALESCE([TITLE], 'User') as [TITLE]
                                        from [bbab].[dbo].[Principal] as a
                                        left join [bbab].[dbo].[Admin] as a
                                        where [PRINCIPAL_ID] = @principalid";

                    //add in parameters to the sql script
                    this.logger.Info("Binding data to Sql");
                    command.Parameters.Add("@principalid", SqlDbType.Int).Value = id;

                    //prepare the statement
                    this.logger.Info("Preparing command");
                    command.Prepare();

                    //read the data recieved
                    this.logger.Info("Executing command ExecuteReader");
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //Don't read if there are no rows
                        this.logger.Info("Checing if data exists");
                        if (reader.HasRows)
                        {
                            //Instantiate model if row is returned.
                            temp = new PrincipalModel();

                            //Read the next line
                            this.logger.Info("Getting the data");
                            reader.Read();

                            //Populate the model
                            temp._id = reader.GetInt32(0);
                            temp._fullName = reader.GetString(1);
                            temp._userName = reader.GetString(2);
                            temp._credentials._email = reader.GetString(3);
                            temp._accountConfirmed = reader.GetByte(4) > 0;
                            temp._phoneNumber = reader.GetString(5);
                        }
                        else
                        {
                            this.logger.Info("Closing the reader");
                            reader.Close();

                            this.logger.Info("Throwing RecordNotFoundException");
                            throw new RecordNotFoundException("User not found.");
                        }

                        //Close the reader
                        this.logger.Info("Closing the reader");
                        reader.Close();
                    }
                }

                //return the model
                this.logger.Info("Returning user model from AccountData@ReadTById");
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
         * <inheritdoc/>
         * <remarks>This instance of this method finds user using email and only email.</remarks>
         * <see cref="ICrud{T}"/>
         */
        public PrincipalModel ReadTByField(PrincipalModel model)
        {
            this.logger.Info("Entering AccountData@ReadTByField");
            try
            {
                //Create temp model to store the data
                PrincipalModel temp;

                //Create the connection and command
                this.logger.Info("Creating SqlCommand");
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //write the sql script to the command
                    this.logger.Info("Generating Sql Script");
                    command.CommandText = @"select p.[PRINCIPAL_ID], [FULL_NAME], [USER_NAME], [EMAIL], [EMAIL_CONFIRMED], [SALT], [PHONE_NUMBER], [PASSWORD], coalesce([LEVEL], 0) as [LEVEL], COALESCE([TITLE], 'User') as [TITLE]
                                        from [bbab].[dbo].[Principal] as p
                                        left join [bbab].[dbo].[Admin] as a
                                        on p.[PRINCIPAL_ID] = a.[PRINCIPAL_ID]
                                        where [EMAIL] = @email";

                    //add in parameters to the sql script
                    this.logger.Info("Binding data to Sql");
                    command.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = model._credentials._email;

                    //prepare the statement
                    this.logger.Info("Preparing command");
                    command.Prepare();

                    //read the data recieved
                    this.logger.Info("Executing command ExecuteReader");
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //Don't read if there are no rows
                        this.logger.Info("Checking if data exists");
                        if (reader.HasRows)
                        {
                            //Instantiate model if row is returned.
                            temp = new PrincipalModel();

                            //Read the next line
                            this.logger.Info("Reading the data");
                            reader.Read();

                            //Populate the model
                            temp._id = reader.GetInt32(0);
                            temp._fullName = reader.GetString(1);
                            temp._userName = reader.GetString(2);
                            temp._credentials._email = reader.GetString(3);
                            temp._accountConfirmed = reader.GetBoolean(reader.GetOrdinal("EMAIL_CONFIRMED"));
                            temp._salt = reader.GetString(5);
                            temp._phoneNumber = reader.GetString(6);
                            temp._credentials._password = reader.GetString(7);
                            temp._adminLevel = reader.GetInt32(8);
                            temp._adminTitle = reader.GetString(9);
                        }
                        else
                        {
                            this.logger.Info("Closing the reader");
                            reader.Close();

                            this.logger.Info("Throwing RecordNotFoundException");
                            throw new RecordNotFoundException("User not found.");
                        }

                        //Close the reader
                        this.logger.Info("Closing the reader");
                        reader.Close();
                    }
                }

                //return the model
                this.logger.Info("Returning user model from AccountData@ReadTByField");
                return temp;
                
            }
            catch(Exception e)
            {
                this.logger.Error("Catching Exception", e);
                this.logger.Info("throwing RecordNotFoundException");
                throw new RecordNotFoundException("User not found", e.InnerException);
            }
        }

        /**
         * <inheritdoc/>
         * <see cref="ICrud{T}"/>
         */
        public void UpdateT(PrincipalModel model)
        {
            this.logger.Info("Entering AccountData@UpdateT");
            try
            {
                //Create the connection and command
                this.logger.Info("Creating SqlCommand");
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Generate the command sql statement
                    this.logger.Info("Generaing Sql Script");
                    command.CommandText = @"UPDATE [bbab].[dbo].[Principal] SET
                        [FULL_NAME] = @fullname,
                        [USER_NAME] = @username,
                        [EMAIL] = @email,
                        [EMAIL_CONFIRMED] = @confirmed,
                        [PHONE_NUMBER] = @phonenumber,
                        [PASSWORD] = @password
                        WHERE [PRINCIPAL_ID] = @userid";

                    //Add parameters to the statement
                    this.logger.Info("Binding Data to sql");
                    command.Parameters.Add("@fullname", SqlDbType.NVarChar, 50).Value = model._fullName;
                    command.Parameters.Add("@username", SqlDbType.NVarChar, 20).Value = model._userName;
                    command.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = model._credentials._email;
                    command.Parameters.Add("@confirmed", SqlDbType.Bit).Value = model._accountConfirmed ? 1 : 0;
                    command.Parameters.Add("phonenumber", SqlDbType.NVarChar, 10).Value = model._phoneNumber;
                    command.Parameters.Add("@password", SqlDbType.NVarChar, 64).Value = model._credentials._password;
                    command.Parameters.Add("@userid", SqlDbType.Int).Value = model._id;

                    //Prepare the statement
                    this.logger.Info("Preparing the command");
                    command.Prepare();

                    //Execute the query
                    this.logger.Info("Executing command NonQuery");
                    command.ExecuteNonQuery();
                }

                this.logger.Info("Exiting AccountData@UpdateT");
            }
            catch(Exception e)
            {
                this.logger.Error("Catching Exception", e);
                this.logger.Info("Throwing RecordNotUpdatedException");
                throw new RecordNotUpdatedException("Profile was not updated.", e.InnerException);
            }
        }
    }
}