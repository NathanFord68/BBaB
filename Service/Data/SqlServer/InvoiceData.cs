using BBaB.Models;
using BBaB.Service.CrudExceptions.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BBaB.Service.Data
{
    public class InvoiceData : ICrud<InvoiceModel>
    {
        private SqlConnection _connection;

        public InvoiceData(SqlConnection connection)
        {
            this._connection =  connection;
        }

        public void CreateT(InvoiceModel model)
        {
            try
            {
                //Create command
                using (SqlCommand command = _connection.CreateCommand())
                {
                    ///Create the first query statement and insert using the binded paramters.
                    ///

                    //Genereate sql script into command
                    command.CommandText = @"INSERT INTO [bbab].[dbo].[Invoice]
                                            ([PRINCIPAL_ID], [ADDRESS_ID], [PRICE], [DATE])
                                            VALUES (@pid, @aid, @price, @date)";

                    //Add parameters to the command string
                    command.Parameters.Add("@pid", SqlDbType.Int).Value = model._customer._id;
                    command.Parameters.Add("@aid", SqlDbType.Int).Value = model._address._id;
                    command.Parameters.Add("@price", SqlDbType.Float).Value = model._totalValue;
                    command.Parameters.Add("@date", SqlDbType.DateTime).Value = model._dateTime;

                    //Prepare the statement
                    command.Prepare();

                    //Execute the command
                    command.ExecuteNonQuery();

                    //Clear command paramters for next query
                    command.Parameters.Clear();

                    ///Prepare the reinsert weapons statement
                    ///
                    command.CommandText = @"INSERT INTO [bbab].[dbo].[SoldWeapons] 
                                            ([MAKE], [MODEL], [CALIBER], [SERIAL_NUMBER], [PRICE])
                                             VALUES (@make, @model, @caliber, @serial, @price);
                                            INSERT INTO [bbab].[dbo].[InvoiceContent]
                                            ([SWEAPON_ID], [INVOICE_ID]) VALUES (@swid, @iid);";
                    foreach (WeaponModel w in model._weapons)
                    {
                        command.Parameters.Add("@make", SqlDbType.NVarChar, 50).Value = w._make;
                        command.Parameters.Add("@model", SqlDbType.NVarChar, 50).Value = w._model;
                        command.Parameters.Add("@caliber", SqlDbType.NVarChar, 10).Value = w._caliber;
                        command.Parameters.Add("@serial", SqlDbType.NVarChar, 25).Value = w._serialNumber;
                        command.Parameters.Add("@price", SqlDbType.Float).Value = w._price;
                        command.Parameters.Add("@swid", SqlDbType.Int).Value = w._id;
                        command.Parameters.Add("@iid", SqlDbType.Int).Value = model._id;

                        //Prepare the statement
                        command.Prepare();

                        //Execute query
                        command.ExecuteNonQuery();

                        //Clear the paramters
                        command.Parameters.Clear();
                    }
                }
            }
            catch (Exception e)
            {
                throw new RecordNotCreatedException("No Invoice was created.", e.InnerException);
            }
        }

        public void DeleteT(InvoiceModel model)
        {
            try
            {
                //Create command
                using (SqlCommand command = _connection.CreateCommand())
                {
                    ///Create the first query statement and insert using the binded paramters.
                    ///

                    //Genereate sql script into command
                    command.CommandText = @"DELETE FROM [bbab].[dbo].[invoicecontent] WHERE [INVOICE_ID] = @id";

                    //Add parameters to the command string
                    command.Parameters.Add("@id", SqlDbType.Int).Value = model._id;

                    //Prepare the statement
                    command.Prepare();

                    //Execute the command
                    command.ExecuteNonQuery();

                    //Clear command paramters for next query
                    command.Parameters.Clear();

                    ///Create the second query statement and insert bridge table using the binded paramters.
                    ///
                    command.CommandText = @"DELETE FROM [bbab].[dbo].[invoice] WHERE [INVOICE_ID] = @id";

                    //Bind id fields for each row in bridge table
                    command.Parameters.Add("@id", SqlDbType.Int).Value = model._id;

                    //Prepare the new statement
                    command.Prepare();

                    //Execute the new command(s)
                    command.ExecuteNonQuery();

                    //Clear command parameters for next iteration
                    command.Parameters.Clear();

                }
            }
            catch (Exception e)
            {
                throw new RecordNotDeletedException("Invoice not deleted.", e.InnerException);
            }
        }

        public List<InvoiceModel> ReadAllT()
        {
            try
            {
                //Create temp list to be returned
                List<InvoiceModel> invoices = new List<InvoiceModel>();

                //Create temp model to populate before adding to the list
                InvoiceModel temp = new InvoiceModel();

                //Create command
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Genereate sql script into command
                    command.CommandText = @"SELECT i.[INVOICE_ID], p.[FULL_NAME], p.[EMAIL], p.[PHONE_NUMBER], 
		                                    a.[ADDRESS_1], a.[ADDRESS_2], a.[ADDRESS_3], a.[CITY], a.[STATE], a.[COUNTRY], a.[ZIP],
		                                    w.[WEAPON_ID], w.[MAKE], w.[MODEL], w.[CALIBER], w.[SERIAL_NUMBER], w.[PRICE] as [WEAPON_PRICE],
		                                    i.[PRICE], i.[DATE]
		                                    FROM [bbab].[dbo].[Invoice] as i
		                                    LEFT JOIN [bbab].[dbo].[InvoiceContent] as ic 
			                                    INNER JOIN [bbab].[dbo].[SWeapon] as w 
			                                        ON ic.[SWEAPON_ID] = w.[SWEAPON_ID]
		                                        ON ic.[INVOICE_ID] = i.[INVOICE_ID]
		                                    LEFT JOIN [bbab].[dbo].[Principal] as p
		                                        ON p.[PRINCIPAL_ID] = i.[PRINCIPAL_ID]
		                                    LEFT JOIN [bbab].[dbo].[Address] as a
		                                        ON a.[ADDRESS_ID] = i.[ADDRESS_ID]";

                    //Prepare the statement
                    command.Prepare();

                    //Execute and Read the data recieved
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //Check the query returned data
                        if (reader.HasRows)
                        {
                            //read each line
                            while (reader.Read())
                            {
                                //Add the last temp model to array if the new row is a different id
                                if(invoices[invoices.Count -1]._id != reader.GetInt32(0))
                                {
                                    invoices.Add(temp);
                                }
                                //check the doesn't line references the same invoice
                                //Make new single data if it doesn't
                                if(temp._id != reader.GetInt32(0))
                                {
                                    //Make a new model
                                    temp = new InvoiceModel();

                                    //Get id field
                                    temp._id = reader.GetInt32(0);

                                    //Get Principal model
                                    temp._customer = new PrincipalModel();
                                    temp._customer._fullName = reader.GetString(1);
                                    temp._customer._credentials._email = reader.GetString(2);
                                    temp._customer._phoneNumber = reader.GetString(3);

                                    //Get Address model
                                    temp._address = new AddressModel();
                                    temp._address._address1 = reader.GetString(4);
                                    temp._address._address2 = reader.GetString(5);
                                    temp._address._address3 = reader.GetString(6);
                                    temp._address._city = reader.GetString(7);
                                    temp._address._state = reader.GetString(8);
                                    temp._address._country = reader.GetString(9);
                                    temp._address._zip = reader.GetString(10);

                                    //Get weapon model
                                    temp._weapons.Add(new WeaponModel(
                                        reader.GetInt32(11),
                                        reader.GetString(12),
                                        reader.GetString(13),
                                        reader.GetString(14),
                                        reader.GetString(15),
                                        reader.GetFloat(16)));

                                    //Get the date and total price
                                    temp._totalValue = reader.GetFloat(17);
                                    temp._dateTime = reader.GetDateTime(18);
                                }
                                else
                                {
                                    //Get weapon model
                                    temp._weapons.Add(new WeaponModel(
                                        reader.GetInt32(11),
                                        reader.GetString(12),
                                        reader.GetString(13),
                                        reader.GetString(14),
                                        reader.GetString(15),
                                        reader.GetFloat(16)));
                                }
                            }
                        }
                        //Close the reader
                        reader.Close();
                    }
                }
                //Return the model
                return invoices;
            }
            catch(Exception e)
            {
                throw new RecordNotFoundException("There were no records found to return.", e.InnerException);
            }
        }

        public List<InvoiceModel> ReadBetweenT(int low, int high)
        {
            try
            {
                //Create temp list to be returned
                List<InvoiceModel> invoices = new List<InvoiceModel>();

                //Create temp model to populate before adding to the list
                InvoiceModel temp = new InvoiceModel();

                //Create command
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Genereate sql script into command
                    command.CommandText = @"SELECT i.[INVOICE_ID], p.[FULL_NAME], p.[EMAIL], p.[PHONE_NUMBER], 
		                                    a.[ADDRESS_1], a.[ADDRESS_2], a.[ADDRESS_3], a.[CITY], a.[STATE], a.[COUNTRY], a.[ZIP],
		                                    w.[WEAPON_ID], w.[MAKE], w.[MODEL], w.[CALIBER], w.[SERIAL_NUMBER], w.[PRICE] as [WEAPON_PRICE],
		                                    i.[PRICE], i.[DATE]
		                                    FROM [bbab].[dbo].[Invoice] as i
		                                    LEFT JOIN [bbab].[dbo].[InvoiceContent] as ic 
			                                    INNER JOIN [bbab].[dbo].[Weapon] as w 
			                                        ON ic.[SWEAPON_ID] = w.[SWEAPON_ID]
		                                        ON ic.[INVOICE_ID] = i.[INVOICE_ID]
		                                    LEFT JOIN [bbab].[dbo].[Principal] as p
		                                        ON p.[PRINCIPAL_ID] = i.[PRINCIPAL_ID]
		                                    LEFT JOIN [bbab].[dbo].[Address] as a
		                                        ON a.[ADDRESS_ID] = i.[ADDRESS_ID]
                                            WHERE i.[INVOICE_ID] BETWEEN @low AND @high";

                    //Bind the data
                    command.Parameters.Add("@low", SqlDbType.Int).Value = low;
                    command.Parameters.Add("@high", SqlDbType.Int).Value = high;

                    //Prepare the statement
                    command.Prepare();

                    //Execute and Read the data recieved
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //Check the query returned data
                        if (reader.HasRows)
                        {
                            //read each line
                            while (reader.Read())
                            {
                                //Add the last temp model to array if the new row is a different id
                                if (invoices[invoices.Count - 1]._id != reader.GetInt32(0))
                                {
                                    invoices.Add(temp);
                                }
                                //check the doesn't line references the same invoice
                                //Make new single data if it doesn't
                                if (temp._id != reader.GetInt32(0))
                                {
                                    //Make a new model
                                    temp = new InvoiceModel();

                                    //Get id field
                                    temp._id = reader.GetInt32(0);

                                    //Get Principal model
                                    temp._customer = new PrincipalModel();
                                    temp._customer._fullName = reader.GetString(1);
                                    temp._customer._credentials._email = reader.GetString(2);
                                    temp._customer._phoneNumber = reader.GetString(3);

                                    //Get Address model
                                    temp._address = new AddressModel();
                                    temp._address._address1 = reader.GetString(4);
                                    temp._address._address2 = reader.GetString(5);
                                    temp._address._address3 = reader.GetString(6);
                                    temp._address._city = reader.GetString(7);
                                    temp._address._state = reader.GetString(8);
                                    temp._address._country = reader.GetString(9);
                                    temp._address._zip = reader.GetString(10);

                                    //Get weapon model
                                    temp._weapons.Add(new WeaponModel(
                                        reader.GetInt32(11),
                                        reader.GetString(12),
                                        reader.GetString(13),
                                        reader.GetString(14),
                                        reader.GetString(15),
                                        reader.GetFloat(16)));

                                    //Get the date and total price
                                    temp._totalValue = reader.GetFloat(17);
                                    temp._dateTime = reader.GetDateTime(18);
                                }
                                else
                                {
                                    //Get weapon model
                                    temp._weapons.Add(new WeaponModel(
                                        reader.GetInt32(11),
                                        reader.GetString(12),
                                        reader.GetString(13),
                                        reader.GetString(14),
                                        reader.GetString(15),
                                        reader.GetFloat(16)));
                                }
                            }
                        }
                        //Close the reader
                        reader.Close();
                    }
                }
                //Return the model
                return invoices;
            }
            catch(Exception e)
            {
                throw new RecordNotFoundException("No invoices found between " + low + " and " + high + ".", e.InnerException);
            }
        }

        /**
         * <summary>Search for an invoice based off an email address.</summary>
         */
        public InvoiceModel ReadTByField(InvoiceModel model)
        {
            try
            {
                //Create temp model to populate before adding to the list
                InvoiceModel temp = new InvoiceModel();

                //Create command
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Genereate sql script into command
                    command.CommandText = @"SELECT i.[INVOICE_ID], p.[FULL_NAME], p.[EMAIL], p.[PHONE_NUMBER], 
		                                    a.[ADDRESS_1], a.[ADDRESS_2], a.[ADDRESS_3], a.[CITY], a.[STATE], a.[COUNTRY], a.[ZIP],
		                                    w.[WEAPON_ID], w.[MAKE], w.[MODEL], w.[CALIBER], w.[SERIAL_NUMBER], w.[PRICE] as [WEAPON_PRICE],
		                                    i.[PRICE], i.[DATE]
		                                    FROM [bbab].[dbo].[Invoice] as i
		                                    LEFT JOIN [bbab].[dbo].[InvoiceContent] as ic 
			                                    INNER JOIN [bbab].[dbo].[Weapon] as w 
			                                        ON ic.[SWEAPON_ID] = w.[SWEAPON_ID]
		                                        ON ic.[INVOICE_ID] = i.[INVOICE_ID]
		                                    LEFT JOIN [bbab].[dbo].[Principal] as p
		                                        ON p.[PRINCIPAL_ID] = i.[PRINCIPAL_ID]
		                                    LEFT JOIN [bbab].[dbo].[Address] as a
		                                        ON a.[ADDRESS_ID] = i.[ADDRESS_ID]
                                            WHERE p.[EMAIL] = @email AND i.[DATE] = @date";

                    //Bind the data
                    command.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = model._customer._credentials._email;
                    command.Parameters.Add("@date", SqlDbType.DateTime).Value = model._dateTime;

                    //Prepare the statement
                    command.Prepare();

                    //Execute and Read the data recieved
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //Check the query returned data
                        if (reader.HasRows)
                        {
                            //read each line
                            while (reader.Read())
                            {
                                //check the doesn't line references the same invoice
                                //Make new single data if it doesn't
                                if (temp._id != reader.GetInt32(0))
                                {
                                    //Make a new model
                                    temp = new InvoiceModel();

                                    //Get id field
                                    temp._id = reader.GetInt32(0);

                                    //Get Principal model
                                    temp._customer = new PrincipalModel();
                                    temp._customer._fullName = reader.GetString(1);
                                    temp._customer._credentials._email = reader.GetString(2);
                                    temp._customer._phoneNumber = reader.GetString(3);

                                    //Get Address model
                                    temp._address = new AddressModel();
                                    temp._address._address1 = reader.GetString(4);
                                    temp._address._address2 = reader.GetString(5);
                                    temp._address._address3 = reader.GetString(6);
                                    temp._address._city = reader.GetString(7);
                                    temp._address._state = reader.GetString(8);
                                    temp._address._country = reader.GetString(9);
                                    temp._address._zip = reader.GetString(10);

                                    //Get weapon model
                                    temp._weapons.Add(new WeaponModel(
                                        reader.GetInt32(11),
                                        reader.GetString(12),
                                        reader.GetString(13),
                                        reader.GetString(14),
                                        reader.GetString(15),
                                        reader.GetFloat(16)));

                                    //Get the date and total price
                                    temp._totalValue = reader.GetFloat(17);
                                    temp._dateTime = reader.GetDateTime(18);
                                }
                                else
                                {
                                    //Get weapon model
                                    temp._weapons.Add(new WeaponModel(
                                        reader.GetInt32(11),
                                        reader.GetString(12),
                                        reader.GetString(13),
                                        reader.GetString(14),
                                        reader.GetString(15),
                                        reader.GetFloat(16)));
                                }
                            }
                        }
                        //Close the reader
                        reader.Close();
                    }
                }
                //Return the model
                return temp;
            }
            catch(Exception e)
            {
                throw new RecordNotFoundException("No records found.", e.InnerException);
            }
        }

        public InvoiceModel ReadTById(int id)
        {
            try
            {
                //Create temp model to populate before adding to the list
                InvoiceModel temp = new InvoiceModel();

                //Create command
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Genereate sql script into command
                    command.CommandText = @"SELECT i.[INVOICE_ID], p.[FULL_NAME], p.[EMAIL], p.[PHONE_NUMBER], 
		                                    a.[ADDRESS_1], a.[ADDRESS_2], a.[ADDRESS_3], a.[CITY], a.[STATE], a.[COUNTRY], a.[ZIP],
		                                    w.[WEAPON_ID], w.[MAKE], w.[MODEL], w.[CALIBER], w.[SERIAL_NUMBER], w.[PRICE] as [WEAPON_PRICE],
		                                    i.[PRICE], i.[DATE]
		                                    FROM [bbab].[dbo].[invoice] as i
		                                    LEFT JOIN [bbab].[dbo].[invoice_content] as ic 
			                                INNER JOIN [bbab].[dbo].[weapon] as w 
			                                ON ic.[WEAPON_ID] = w.[WEAPON_ID]
		                                    ON ic.[INVOICE_ID] = i.[INVOICE_ID]
		                                    LEFT JOIN [bbab].[dbo].[principal] as p
		                                    ON p.[PRINCIPAL_ID] = i.[PRINCIPAL_ID]
		                                    LEFT JOIN [bbab].[dbo].[address] as a
		                                    ON a.[ADDRESS_ID] = i.[ADDRESS_ID]
                                            WHERE i.[INVOICE_ID] = @id";

                    //Bind the data
                    command.Parameters.Add("@email", SqlDbType.Int).Value = id;

                    //Prepare the statement
                    command.Prepare();

                    //Execute and Read the data recieved
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //Check the query returned data
                        if (reader.HasRows)
                        {
                            //read each line
                            while (reader.Read())
                            {
                                //check the doesn't line references the same invoice
                                //Make new single data if it doesn't
                                if (temp._id != reader.GetInt32(0))
                                {
                                    //Make a new model
                                    temp = new InvoiceModel();

                                    //Get id field
                                    temp._id = reader.GetInt32(0);

                                    //Get Principal model
                                    temp._customer = new PrincipalModel();
                                    temp._customer._fullName = reader.GetString(1);
                                    temp._customer._credentials._email = reader.GetString(2);
                                    temp._customer._phoneNumber = reader.GetString(3);

                                    //Get Address model
                                    temp._address = new AddressModel();
                                    temp._address._address1 = reader.GetString(4);
                                    temp._address._address2 = reader.GetString(5);
                                    temp._address._address3 = reader.GetString(6);
                                    temp._address._city = reader.GetString(7);
                                    temp._address._state = reader.GetString(8);
                                    temp._address._country = reader.GetString(9);
                                    temp._address._zip = reader.GetString(10);

                                    //Get weapon model
                                    temp._weapons.Add(new WeaponModel(
                                        reader.GetInt32(11),
                                        reader.GetString(12),
                                        reader.GetString(13),
                                        reader.GetString(14),
                                        reader.GetString(15),
                                        reader.GetFloat(16)));

                                    //Get the date and total price
                                    temp._totalValue = reader.GetFloat(17);
                                    temp._dateTime = reader.GetDateTime(18);
                                }
                                else
                                {
                                    //Get weapon model
                                    temp._weapons.Add(new WeaponModel(
                                        reader.GetInt32(11),
                                        reader.GetString(12),
                                        reader.GetString(13),
                                        reader.GetString(14),
                                        reader.GetString(15),
                                        reader.GetFloat(16)));
                                }
                            }
                        }
                        //Close the reader
                        reader.Close();
                    }
                }
                //Return the model
                return temp;
            }
            catch(Exception e)
            {
                throw new RecordNotFoundException("No record found.", e.InnerException);
            }
        }

        /**
         * <summary>Updates the list of weapons attached to an invoice.</summary>
         * <remarks>This method will delete all the sold weapons attached to 
         * this invoice before inserting new values. Ensure you are passing in the correct 
         * list of weapons before executing this update.</remarks>
         */
        public void UpdateT(InvoiceModel model)
        {
            try
            {
                //Create command
                using (SqlCommand command = _connection.CreateCommand())
                {
                    ///Prepare the delete statement
                    ///
                    //Genereate sql script into command
                    command.CommandText = @"DELETE FROM [bbab].[dbo].[InvoiceContent] where [INVOICE_CONTENT_ID] = @id";

                    //Add parameters to the command string
                    command.Parameters.Add("@id", SqlDbType.Int).Value = model._id;

                    //Prepare the statement
                    command.Prepare();

                    //Execute the command
                    command.ExecuteNonQuery();

                    //Clear the query
                    command.Parameters.Clear();

                    ///Prepare the reinsert weapons statement
                    ///
                    command.CommandText = @"INSERT INTO [bbab].[dbo].[SoldWeapons] 
                                            ([MAKE], [MODEL], [CALIBER], [SERIAL_NUMBER], [PRICE])
                                             VALUES (@make, @model, @caliber, @serial, @price);
                                            INSERT INTO [bbab].[dbo].[InvoiceContent]
                                            ([SWEAPON_ID], [INVOICE_ID]) VALUES (@swid, @iid);";
                    foreach(WeaponModel w in model._weapons)
                    {
                        command.Parameters.Add("@make", SqlDbType.NVarChar, 50).Value = w._make;
                        command.Parameters.Add("@model", SqlDbType.NVarChar, 50).Value = w._model;
                        command.Parameters.Add("@caliber", SqlDbType.NVarChar, 10).Value = w._caliber;
                        command.Parameters.Add("@serial", SqlDbType.NVarChar, 25).Value = w._serialNumber;
                        command.Parameters.Add("@price", SqlDbType.Float).Value = w._price;
                        command.Parameters.Add("@swid", SqlDbType.Int).Value = w._id;
                        command.Parameters.Add("@iid", SqlDbType.Int).Value = model._id;

                        //Prepare the statement
                        command.Prepare();

                        //Execute query
                        command.ExecuteNonQuery();

                        //Clear the paramters
                        command.Parameters.Clear();
                    }
                }
            }
            catch(Exception e)
            {
                throw new RecordNotUpdatedException("Record was not updated.", e.InnerException);
            }
        }
    }
}