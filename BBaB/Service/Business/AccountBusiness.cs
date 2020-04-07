using BBaB.Models;
using BBaB.Service.Data;
using BBaB.Service.Exceptions;
using BBaB.Services.Data;
using BBaB.Utility;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using BBaB.Utility.Interfaces;

namespace BBaB.Service.Business
{
    /**
     * TODO DI the data layer classes
     */
    public class AccountBusiness : IAccountBusiness<PrincipalModel, AddressModel>
    {
        private IDBConnect<SqlConnection> _connect;
        private IBBaBLogger logger;

        public AccountBusiness(IDBConnect<SqlConnection> connect, IBBaBLogger logger)
        {
            this._connect = connect;
            this.logger = logger;
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public void AddToCart(CartModel cartItem)
        {
            this.logger.Info(cartItem._customer._fullName, "Entering AccountBusiness.AddToCart to add an item for");

            this.logger.Info(cartItem._customer._fullName, "Getting a connection.");
            using (SqlConnection connection = _connect.GetConnection())
            {
                this.logger.Info(cartItem._customer._fullName, "Opening a connection to the database");
                connection.Open();

                this.logger.Info(cartItem._customer._fullName, "Getting data layer CartData");
                ICrud<CartModel> cartData = new CartData(connection);

                this.logger.Info(cartItem._customer._fullName, "Passing cart item to be added");
                cartData.CreateT(cartItem);

                this.logger.Info(cartItem._customer._fullName, "Closing the database connection");
                connection.Close();
            }

            this.logger.Info(cartItem._customer._fullName, "Leaving AccountBusiness@AddToCart");
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public PrincipalModel AuthenticateAccount(PrincipalModel model)
        {
            this.logger.Info(model._fullName, "Entering AccountBusiness@AuthenticateAccount");
            PrincipalModel account;
            //Get the connection
            this.logger.Info(model._fullName, "Getting a connection to the database");
            using (SqlConnection connection = _connect.GetConnection())
            {
                this.logger.Info(model._fullName, "Opening the connection to the database");
                connection.Open();
                //Instantiate the data layer
                ICrud<PrincipalModel> accountData = new AccountData(connection);

                //Get the database account for this user.
                account = accountData.ReadTByField(model);

                //Salt the logging in users password
                model._credentials._password = new StringManipulator().HashPassword(account._salt + model._credentials._password);

                //Compare passwords
                if(model._credentials._password.Equals(account._credentials._password))
                {
                    account._salt = null;
                }
                else
                {
                    connection.Close();
                    throw new AuthenticationFailedException("Login was unsuccessful");
                }
                connection.Close();
            }

            //Return the model
            return account;
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public void DeleteAccount(PrincipalModel model)
        {
            using (SqlConnection connection = _connect.GetConnection())
            {
                connection.Open();
                ICrud<PrincipalModel> crud = new AccountData(connection);
                crud.DeleteT(model);

                connection.Close();
            }
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public AddressModel GetAddress(PrincipalModel model)
        {
            //Generate model that needs populating
            AddressModel address = new AddressModel();
            //Store user in model for query
            address._resident = model;
            using (SqlConnection connection = _connect.GetConnection())
            {
                connection.Open();
                //Get the data layer
                ICrud<AddressModel> addressData = new AddressData(connection);

                //Pass the model to get the data
                address = addressData.ReadTByField(address);

                //close the connection
                connection.Close();
            }

            return address;
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public CartModel GetCart(PrincipalModel model)
        {
            //Declare the model
            CartModel cart;

            //Get the connection
            using (SqlConnection connection = _connect.GetConnection())
            {
                connection.Open();
                //Instantiate the data layer
                ICrud<CartModel> cartData = new CartData(connection);

                //Make the cart
                cart = cartData.ReadTById(model._id);

                //Close the connection
                connection.Close();
            }

            //Return the cart
            return cart;
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public InvoiceModel ProcessPurchase(CartModel cart)
        {
            //Create the invoice model
            InvoiceModel invoice = new InvoiceModel();
            //Get connection
            using (SqlConnection connection = _connect.GetConnection())
            {
                connection.Open();
                //Create the transaction
                SqlTransaction transaction;

                //Start the transaction
                transaction = connection.BeginTransaction();

                //Create the data layer
                ICrud<InvoiceModel> invoiceData = new InvoiceData(connection);
                ICrud<CartModel> cartData = new CartData(connection);
                ICrud<AddressModel> addressData = new AddressData(connection);
                ICrud<WeaponModel> weaponData = new WeaponData(connection);

                try
                {
                    //Populate the address in the invoice
                    AddressModel address = new AddressModel();
                    address._resident = cart._customer;

                    invoice._address = addressData.ReadTByField(address);

                    //Populate the customer in the invoice
                    invoice._customer = cart._customer;

                    //Populate the weapons in the invoice
                    invoice._weapons = cart._weapons;

                
                    //Process transaction
                    invoiceData.CreateT(invoice);

                    foreach (WeaponModel w in cart._weapons)
                    {
                        //Assign each weapon to be updated.
                        cart._weaponToUpdate = w;

                        //Update cart and inventory
                        weaponData.DeleteT(cart._weaponToUpdate);
                        cartData.DeleteT(cart);
                    }

                    transaction.Commit();
                }catch(Exception e)
                {
                    transaction.Rollback();
                    connection.Close();
                    throw new TransactionFailedException("The transaction failed.", e.InnerException);
                }
                connection.Close();
            }

            //Return the invoice
            return invoice;
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public void UpdateAccount(PrincipalModel model)
        {
            //Get a connection
            using (SqlConnection connection = _connect.GetConnection())
            {
                connection.Open();
                //Get data layer
                ICrud<PrincipalModel> accountData = new AccountData(connection);

                //Update account
                accountData.UpdateT(model);

                //Close connection
                connection.Close();
            }
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public void RegisterAccount(PrincipalModel model)
        {
            //Get a String Manipulator class
            StringManipulator strManip = new StringManipulator();

            //Get a salt value
            model._salt = strManip.GenerateString(64);

            //Hash password with salt
            model._credentials._password = strManip.HashPassword(model._salt + model._credentials._password);

            //Get connection to database
            using (SqlConnection connection = _connect.GetConnection())
            {
                connection.Open();
                //Get data layer
                ICrud<PrincipalModel> accountData = new AccountData(connection);

                //Send data to be processed
                accountData.CreateT(model);

                //Close the connection
                connection.Close();
            }

        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public void RegisterAddress(AddressModel model)
        {
            //Get a connection
            using (SqlConnection connection = _connect.GetConnection())
            {
                connection.Open();
                //Get the data layer
                ICrud<AddressModel> addressData = new AddressData(connection);

                //Pass model to be stored
                addressData.CreateT(model);

                //Close the connection
                connection.Close();               
            }
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public void RemoveFromCart(WeaponModel weapon)
        {
            //Create the cart with the weapon to be updated
            CartModel cart = new CartModel();
            cart._weaponToUpdate = weapon;

            //Get a connection
            using (SqlConnection connection = _connect.GetConnection())
            {
                connection.Open();
                //Get the data layer
                ICrud<CartModel> cartData = new CartData(connection);

                //Delete the item from the cart
                cartData.DeleteT(cart);

                //Close the connection
                connection.Close();
            }
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public void UpdateAddress(AddressModel model)
        {
            //Get a connection
            using (SqlConnection connection = _connect.GetConnection())
            {
                connection.Open();
                //Get the data layer
                ICrud<AddressModel> addressData = new AddressData(connection);

                //Update the model
                addressData.UpdateT(model);

                //Close the connection
                connection.Close();
            }
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public InvoiceModel ViewInvoice(PrincipalModel customer, DateTime date)
        {
            //Declare the invoice
            InvoiceModel invoice = new InvoiceModel();
            invoice._customer = customer;
            invoice._dateTime = date;

            //Get a connection
            using (SqlConnection connection = _connect.GetConnection())
            {
                connection.Open();
                //Get the data layer
                ICrud<InvoiceModel> invoiceData = new InvoiceData(connection);

                //Get the invoice for this user
                invoice = invoiceData.ReadTByField(invoice);

                //Close the connection
                connection.Close();
            }

            //Return the invoice
            return invoice;
        }

        /**
         * <see cref="IAccountBusiness"/>
         */
        public PrincipalModel UpdatePassword(PrincipalModel model, string newp)
        {
            //Get a connection
            using (SqlConnection connection = _connect.GetConnection())
            {
                //Open the connection
                connection.Open();

                //Get the data layer
                ICrud<PrincipalModel> accountData = new AccountData(connection);

                //Authenticate the old password
                //
                //Get the data profile
                PrincipalModel account = accountData.ReadTByField(model);

                //Create the string manipulator
                StringManipulator sm = new StringManipulator();
                //Hash the old password
                model._credentials._password = sm.HashPassword(account._salt + model._credentials._password);

                //Check passwords match
                if (account._credentials._password.Equals(model._credentials._password))
                {
                    //if they do, update the password
                    model._credentials._password = sm.HashPassword(account._salt + newp);
                    accountData.UpdateT(model);
                    model._salt = "";
                }
                else
                {
                    connection.Close();
                    throw new AuthenticationFailedException("Incorrect old password");
                }

                //close the connection
                connection.Close();
            }

            return model;
        }
    }
}