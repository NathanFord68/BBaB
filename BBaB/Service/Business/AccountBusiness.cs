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
            this.logger.Info(cartItem._customer._credentials._email, "Entering AccountBusiness.AddToCart to add an item for");

            this.logger.Info(cartItem._customer._credentials._email, "Getting a connection.");
            using (SqlConnection connection = _connect.GetConnection())
            {
                this.logger.Info(cartItem._customer._credentials._email, "Opening a connection to the database");
                connection.Open();

                this.logger.Info(cartItem._customer._credentials._email, "Getting data layer CartData");
                ICrud<CartModel> cartData = new CartData(connection);

                this.logger.Info(cartItem._customer._credentials._email, "Passing cart item to be added");
                cartData.CreateT(cartItem);

                this.logger.Info(cartItem._customer._credentials._email, "Closing the database connection");
                connection.Close();
            }

            this.logger.Info(cartItem._customer._credentials._email, "Leaving AccountBusiness@AddToCart");
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public PrincipalModel AuthenticateAccount(PrincipalModel model)
        {
            this.logger.Info(model._credentials._email, "Entering AccountBusiness@AuthenticateAccount");
            PrincipalModel account;
            //Get the connection
            this.logger.Info(model._credentials._email, "Getting a connection to the database");
            using (SqlConnection connection = _connect.GetConnection())
            {
                this.logger.Info(model._credentials._email, "Opening the connection to the database");
                connection.Open();

                //Instantiate the data layer
                this.logger.Info(model._credentials._email, "Getting data layer AccountData");
                ICrud<PrincipalModel> accountData = new AccountData(connection, this.logger);

                //Get the database account for this user.
                this.logger.Info(model._credentials._email, "Getting the recorded model for this user");
                account = accountData.ReadTByField(model);

                //Salt the logging in users password
                this.logger.Info(model._credentials._email, "Salting the inputed password for comparison");
                model._credentials._password = new StringManipulator().HashPassword(account._salt + model._credentials._password);

                //Compare passwords
                this.logger.Info(model._credentials._email, "Comparing the passwords to authenticate account");
                if (model._credentials._password.Equals(account._credentials._password))
                {
                    this.logger.Info(model._credentials._email, "Nullifying salt to prevent it from reaching the presentation layer");
                    account._salt = null;
                }
                else
                {
                    this.logger.Info(model._credentials._email, "Closing the connection before throwing a failed login exception");
                    connection.Close();

                    this.logger.Info(model._credentials._email, "Throwing a login unseccessful exception to signal an unauthenticated user");
                    throw new AuthenticationFailedException("Login was unsuccessful");
                }

                this.logger.Info(model._credentials._email, "Closing the connection to the database");
                connection.Close();

            }

            //Return the model
            this.logger.Info(model._credentials._email, "Returning account model from AccountBusiness@AuthenticateUser with successful authentication");
            return account;
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public void DeleteAccount(PrincipalModel model)
        {
            this.logger.Info(model._credentials._email, "Entering AccountBusiness@DeleteAccount");
            this.logger.Info(model._credentials._email, "Getting a connection to the database");
            using (SqlConnection connection = _connect.GetConnection())
            {
                this.logger.Info(model._credentials._email, "Opening a connection to the database");
                connection.Open();

                this.logger.Info(model._credentials._email, "Getting data layer AccountData");
                ICrud<PrincipalModel> crud = new AccountData(connection, this.logger);

                this.logger.Info(model._credentials._email, "Passing model to be deleted in the data layer");
                crud.DeleteT(model);

                this.logger.Info(model._credentials._email, "Closing connection to the database");
                connection.Close();
            }

            this.logger.Info(model._credentials._email, "Returning from AccountBusiness@DeleteAccount with successful deletion of account");
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public AddressModel GetAddress(PrincipalModel model)
        {
            this.logger.Info(model._credentials._email, "Entering AccountBusiness@GetAddress");

            //Generate model that needs populating
            AddressModel address = new AddressModel();
            //Store user in model for query
            address._resident = model;

            this.logger.Info(model._credentials._email, "Getting a connection to the database");
            using (SqlConnection connection = _connect.GetConnection())
            {
                this.logger.Info(model._credentials._email, "Opening connection to the database");
                connection.Open();
                //Get the data layer

                this.logger.Info(model._credentials._email, "Getting data layer AddressData");
                ICrud<AddressModel> addressData = new AddressData(connection);

                //Pass the model to get the data
                this.logger.Info(model._credentials._email, "Passing model to get the address of this user");
                address = addressData.ReadTByField(address);

                //close the connection
                this.logger.Info(model._credentials._email, "Closing the connection to the database");
                connection.Close();
            }

            this.logger.Info(model._credentials._email, "Returning the address model of the user");
            return address;
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public CartModel GetCart(PrincipalModel model)
        {
            this.logger.Info(model._credentials._email, "Entering AccountBusiness@GetCart");
            //Declare the model
            CartModel cart;

            //Get the connection
            this.logger.Info(model._credentials._email, "Getting a connection to the database");
            using (SqlConnection connection = _connect.GetConnection())
            {
                this.logger.Info(model._credentials._email, "Opening a connection to the database");
                connection.Open();

                //Instantiate the data layer
                this.logger.Info(model._credentials._email, "Getting data layer CartData");
                ICrud<CartModel> cartData = new CartData(connection);

                //Make the cart
                this.logger.Info(model._credentials._email, "Passing model to retrieve the cart data from the data layer");
                cart = cartData.ReadTById(model._id);

                //Close the connection
                this.logger.Info(model._credentials._email, "Closing connection to the database");
                connection.Close();
            }

            //Return the cart
            this.logger.Info(model._credentials._email, "Returning the model from AccountBusiness@GetCart");
            return cart;
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public InvoiceModel ProcessPurchase(CartModel cart)
        {
            this.logger.Info(cart._customer._credentials._email, "Entering AccountBusiness@ProcessPurchase");

            //Create the invoice model
            InvoiceModel invoice = new InvoiceModel();

            //Get connection
            this.logger.Info(cart._customer._credentials._email, "Getting a connection to the database");
            using (SqlConnection connection = _connect.GetConnection())
            {
                this.logger.Info(cart._customer._credentials._email, "Opening connection to the database");
                connection.Open();

                //Create the transaction
                this.logger.Info(cart._customer._credentials._email, "Declaring a transaction for the database");
                SqlTransaction transaction;

                //Start the transaction
                this.logger.Info(cart._customer._credentials._email, "Initializing/Beginning transaction");
                transaction = connection.BeginTransaction();

                //Create the data layer
                this.logger.Info(cart._customer._credentials._email, "Getting data layer InvoiceData, CartData, AddressData, WeaponData");
                ICrud<InvoiceModel> invoiceData = new InvoiceData(connection);
                ICrud<CartModel> cartData = new CartData(connection);
                ICrud<AddressModel> addressData = new AddressData(connection);
                ICrud<WeaponModel> weaponData = new WeaponData(connection);

                try
                {
                    //Populate the address in the invoice
                    this.logger.Info(cart._customer._credentials._email, "Populating address information of the invoice");
                    AddressModel address = new AddressModel();
                    address._resident = cart._customer;

                    invoice._address = addressData.ReadTByField(address);

                    //Populate the customer in the invoice
                    this.logger.Info(cart._customer._credentials._email, "Populating customer information of the invoice");
                    invoice._customer = cart._customer;

                    //Populate the weapons in the invoice
                    this.logger.Info(cart._customer._credentials._email, "Populating the weapon information of the invoice");
                    invoice._weapons = cart._weapons;


                    //Process transaction
                    this.logger.Info(cart._customer._credentials._email, "Passing invoice record to the data layer");
                    invoiceData.CreateT(invoice);

                    this.logger.Info(cart._customer._credentials._email, "Looping through Weaopon list to Move item Delete from inventory");
                    foreach (WeaponModel w in cart._weapons)
                    {
                        //Assign each weapon to be updated.
                        cart._weaponToUpdate = w;

                        //Update cart and inventory
                        weaponData.DeleteT(cart._weaponToUpdate);
                    }

                    this.logger.Info(cart._customer._credentials._email, "Deleting cart object");
                    cartData.DeleteT(cart);

                    this.logger.Info(cart._customer._credentials._email, "Commiting the transaction");
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    this.logger.Error(cart._customer._credentials._email, "Catching Exception ", e);

                    this.logger.Warning(cart._customer._credentials._email, "Rolling back transaction");
                    transaction.Rollback();

                    this.logger.Info(cart._customer._credentials._email, "Closing connection to database after failed transaction");
                    connection.Close();

                    this.logger.Info(cart._customer._credentials._email, "Throwing exception back to the presentation layer");
                    throw new TransactionFailedException("The transaction failed.", e.InnerException);
                }
                this.logger.Info(cart._customer._credentials._email, "Closing connection to the database");
                connection.Close();
            }

            //Return the invoice
            this.logger.Info(cart._customer._credentials._email, "Returning invoice model after a successful transaction in AccountBusiness@ProcessPurchase");
            return invoice;
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public void UpdateAccount(PrincipalModel model)
        {
            this.logger.Info(model._credentials._email, "Entering AccountBusiness@UpdateAccount");

            //Get a connection
            this.logger.Info(model._credentials._email, "Getting a connection to the database");
            using (SqlConnection connection = _connect.GetConnection())
            {
                this.logger.Info(model._credentials._email, "Opening connection to the database");
                connection.Open();

                //Get data layer
                this.logger.Info(model._credentials._email, "Getting data lyaer AccountData");
                ICrud<PrincipalModel> accountData = new AccountData(connection, this.logger);

                //Update account
                this.logger.Info(model._credentials._email, "Passing model to be updated in data layer");
                accountData.UpdateT(model);

                //Close connection
                this.logger.Info(model._credentials._email, "Closing connection to the database");
                connection.Close();
            }
            this.logger.Info(model._credentials._email, "Exiting AccountBusiness@UpdateAccount");
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public void RegisterAccount(PrincipalModel model)
        {
            this.logger.Info(model._credentials._email, "Entering AccountBusiness@RegisterAccount");

            //Get a String Manipulator class
            StringManipulator strManip = new StringManipulator();

            //Get a salt value
            this.logger.Info(model._credentials._email, "Creating a new salt value for account");
            model._salt = strManip.GenerateString(64);

            //Hash password with salt
            this.logger.Info(model._credentials._email, "Hashing password with salt");
            model._credentials._password = strManip.HashPassword(model._salt + model._credentials._password);

            //Get connection to database
            this.logger.Info(model._credentials._email, "Getting a connection to the database");
            using (SqlConnection connection = _connect.GetConnection())
            {
                this.logger.Info(model._credentials._email, "Opening connection to database");
                connection.Open();

                //Get data layer
                this.logger.Info(model._credentials._email, "Getting data layer AccountData");
                ICrud<PrincipalModel> accountData = new AccountData(connection, this.logger);

                //Send data to be processed
                this.logger.Info(model._credentials._email, "Passing model to be registered in data layer");
                accountData.CreateT(model);

                //Close the connection
                this.logger.Info(model._credentials._email, "Closing connection to the database");
                connection.Close();
            }

            this.logger.Info(model._credentials._email, "Exiting AccountBusiness@RegisterAccount");
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public void RegisterAddress(AddressModel model)
        {
            this.logger.Info(model._resident._credentials._email, "Entering AccountBusiness@RegisterAddress");

            //Get a connection
            this.logger.Info(model._resident._credentials._email, "Getting a connection to the database");
            using (SqlConnection connection = _connect.GetConnection())
            {
                this.logger.Info(model._resident._credentials._email, "Opening a connection to the database");
                connection.Open();

                //Get the data layer
                this.logger.Info(model._resident._credentials._email, "Getting data layer AddressData");
                ICrud<AddressModel> addressData = new AddressData(connection);

                //Pass model to be stored
                this.logger.Info(model._resident._credentials._email, "Passing model to the data layer");
                addressData.CreateT(model);

                //Close the connection
                this.logger.Info(model._resident._credentials._email, "Closing connection to the database");
                connection.Close();
            }
            this.logger.Info(model._resident._credentials._email, "Exiting AccountBusiness@RegisterAddress");
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        //TODO fix this method
        public void RemoveFromCart(WeaponModel weapon)
        {
            /*
             * Reha if your looking at this. I'm ommiting logging statements cause I 
             * realized this method wouldn't work even if we used it. I'm adding it 
             * to list of things to do for the clean up milestone
             */
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
            this.logger.Info(model._resident._credentials._email, "Entering AccountBusiness@UpdateAddress");

            //Get a connection
            this.logger.Info(model._resident._credentials._email, "Getting a connection to the database");
            using (SqlConnection connection = _connect.GetConnection())
            {
                this.logger.Info(model._resident._credentials._email, "Opening connection to the database");
                connection.Open();

                //Get the data layer
                this.logger.Info(model._resident._credentials._email, "Getting data layer AddressData");
                ICrud<AddressModel> addressData = new AddressData(connection);

                //Update the model
                this.logger.Info(model._resident._credentials._email, "Passing model to be updated");
                addressData.UpdateT(model);

                //Close the connection
                this.logger.Info(model._resident._credentials._email, "Closing connection to the database");
                connection.Close();
            }
            this.logger.Info(model._resident._credentials._email, "Exiting AccountBusiness@UpdateAddress");
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public InvoiceModel ViewInvoice(PrincipalModel customer, DateTime date)
        {
            this.logger.Info(customer._credentials._email, "Entering AccountBusiness@ViewInvoice");

            //Declare the invoice
            InvoiceModel invoice = new InvoiceModel();
            invoice._customer = customer;
            invoice._dateTime = date;

            //Get a connection
            this.logger.Info(customer._credentials._email, "Getting connection to the database");
            using (SqlConnection connection = _connect.GetConnection())
            {
                this.logger.Info(customer._credentials._email, "Opening connection to the database");
                connection.Open();

                //Get the data layer
                this.logger.Info(customer._credentials._email, "Getting data layer InvoiceData");
                ICrud<InvoiceModel> invoiceData = new InvoiceData(connection);

                //Get the invoice for this user
                this.logger.Info(customer._credentials._email, "Retriving the invoice from the data layer");
                invoice = invoiceData.ReadTByField(invoice);

                //Close the connection
                this.logger.Info(customer._credentials._email, "Closing connection to the database");
                connection.Close();
            }

            //Return the invoice
            this.logger.Info(customer._credentials._email, "Returning invoice model from AccountBusiness@ViewInvoice");
            return invoice;
        }

        /**
         * <see cref="IAccountBusiness"/>
         */
        public PrincipalModel UpdatePassword(PrincipalModel model, string newp)
        {
            this.logger.Info(model._credentials._email, "Entering AccountBusiness@UpdatePassword");

            //Get a connection
            this.logger.Info(model._credentials._email, "Getting connection to the database");
            using (SqlConnection connection = _connect.GetConnection())
            {
                //Open the connection
                this.logger.Info(model._credentials._email, "Opening connection to the database");
                connection.Open();

                //Get the data layer
                this.logger.Info(model._credentials._email, "Getting data layer AccountData");
                ICrud<PrincipalModel> accountData = new AccountData(connection, this.logger);

                ///Authenticate the old password
                //
                //Get the data profile
                this.logger.Info(model._credentials._email, "Getting users old information from the data layer");
                PrincipalModel account = accountData.ReadTByField(model);

                //Create the string manipulator
                StringManipulator sm = new StringManipulator();
                //Hash the old password
                this.logger.Info(model._credentials._email, "Hashing the user's old password");
                model._credentials._password = sm.HashPassword(account._salt + model._credentials._password);

                //Check passwords match
                this.logger.Info(model._credentials._email, "Confirmin password match");
                if (account._credentials._password.Equals(model._credentials._password))
                {
                    //if they do, update the password
                    this.logger.Info(model._credentials._email, "Update user's account with new password");
                    model._credentials._password = sm.HashPassword(account._salt + newp);

                    this.logger.Info(model._credentials._email, "Passing model to be updated in the database");
                    accountData.UpdateT(model);

                    this.logger.Info(model._credentials._email, "Set salt value to empty to keep it from presentation layer");
                    model._salt = "";
                }
                else
                {
                    this.logger.Info(model._credentials._email, "Closing connection to the database after failed authentication");
                    connection.Close();

                    this.logger.Info(model._credentials._email, "Throwing exception to presentation layer");
                    throw new AuthenticationFailedException("Incorrect old password");
                }

                //close the connection
                this.logger.Info(model._credentials._email, "Closing connection to the database");
                connection.Close();
            }

            this.logger.Info(model._credentials._email, "Returning new model from AccountBusiness@UpdatePassword");
            return model;
        }
    }
}