using BBaB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBaB.Service.Business
{
    /**
     * <typeparam name="P">For user account</typeparam>
     * <typeparam name="A">For user Address</typeparam>
     */
    public interface IAccountBusiness<P, A>
    {
        /**
         * <summary>Authenticate a user's email and password</summary>
         * <param name="model" type="P"></param>
         * <returns type="T"></returns>
         * <exception cref="AuthenticationFailedException"></exception>
         */
        P AuthenticateAccount(P model);

        /**
         * <summary>Prepares user model fo registration</summary>
         * <param name="model" type="P"></param>
         */
        void RegisterAccount(P model);

        /**
         * <summary>Delete an account with it's address</summary>
         * <param name="model" type="P"></param>
         */
        void DeleteAccount(P model);

        /**
         * <summary>Update a user account</summary>
         * <param name="model" type="P"></param>
         */
        void UpdateAccount(P model);

        /**
         * <summary>Updates a users password</summary>
         * <param name="model" type="P"></param>
         * <param name="newp" type="string"></param>
         */
        P UpdatePassword(P model, string newp);

        /**
         * <summary>Register an address</summary>
         * <param name="model" type="A"></param>
         */
        void RegisterAddress(A model);

        /**
         * <summary>Get a users address.</summary>
         * <param name="model" type="P"></param>
         * <remarks>Ensure the parameter value is the account id</remarks>
         */
        A GetAddress(P model);

        /**
         * <summary>Update the address associated with an account. Ensure the principal id is set.</summary>
         * <param name="model" type="A"></param>
         */
        void UpdateAddress(A model);

        /**
         * <summary>Adds and item to the accounts cart</summary>
         * TODO figure out what parameter this needs
         */
        void AddToCart(CartModel cartItem);

        /**
         * <summary>Removes an item from the accounts cart</summary>
         * TODO figure out what parameter this needs.
         */
        void RemoveFromCart(WeaponModel weapon);

        /**
         * <summary>Process generation of the cart invoice after payment</summary>
         * TODO figure out paramter
         * <returns type="InvoiceModel"></returns>
         */
        InvoiceModel ProcessPurchase(CartModel cart);

        /**
         * <summary>Gets the cart.</summary>
         * TODO figure out parameter
         * <returns type="CartModel"></returns>
         */
        CartModel GetCart(P model);

        /**
         * <summary>View the invoice for a given account email and date</summary>
         * TODO figure out parameter
         * <returns type="InvoiceModel"></returns>
         */
        InvoiceModel ViewInvoice(P custumer, DateTime date);
    }
}
