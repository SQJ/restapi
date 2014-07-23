//-----------------------------------------------------------------------
// <copyright file="IBdk.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>The interface is used for SCS unit test support, so we can replace the implementation of SCS proxy in unit test.</summary>
// <author>v-weizho</author>
//-----------------------------------------------------------------------
namespace MS.Support.CMATGateway.Proxy.SCS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The interface is used for SCS unit test support, so we can replace the implementation of SCS proxy in unit test.
    /// </summary>
    public interface IBdk
    {
        /// <summary>
        /// Retrieves the public encryption key that partners use to encrypt sensitive payment instrument data such as credit card numbers, direct debit account numbers, and Qwest billing telephone numbers (BTNs).
        /// </summary>
        /// <param name="lRequesterIdHigh">Agent PUID high value.</param>
        /// <param name="lRequesterIdLow">Agent PUID low value.</param>
        /// <param name="bstrKeyGuid">The GUID of the public key. Set this parameter to 89BA9D19-9A04-4A15-A2A3-AF881B10FF53.</param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used.</param>
        /// <param name="pbstrKeyXml">The public key to use for encryption.</param>
        void GetKey(int lRequesterIdHigh, int lRequesterIdLow, string bstrKeyGuid, out string pbstrErrorXML, out string pbstrKeyXml);

        /// <summary>
        /// Searches accounts based on selected criteria.
        /// </summary>
        /// <param name="delegatePuidHigh">The PUIDHigh of the person making the request.</param>
        /// <param name="delegatePuidLow">The PUIDLow of the person making the request.</param>
        /// <param name="accountSearchCriteriaXml">The criteria of the account for which the search is made.</param>
        /// <param name="bookmark">The encoded string that determines where to begin the search.</param>
        /// <param name="returnedAccountCountMax">The number of account records to retrieve. This must be between 1 and 50. </param>
        /// <param name="bookmarkNew">The encoded string that determines where the next call to the SearchAccountsEx method will begin searching.</param>
        /// <param name="returnedAccountCount">The number of account records returned. This value will be between 0 and the value passed in the returnedAccountCountMax parameter.</param>
        /// <param name="accountInfoSetXml">Returns a subset of the details for each account.</param>
        void SearchAccountsEx(int delegatePuidHigh, int delegatePuidLow, string accountSearchCriteriaXml, string bookmark, int returnedAccountCountMax, out string bookmarkNew, out int returnedAccountCount, out string accountInfoSetXml);

        /// <summary>
        /// Retrieves OPEN, LOCKED, or CLOSED accounts for which the supplied search PUID has an input role.
        /// </summary>
        /// <param name="delegateIdHigh">Agent PUID high value.</param>
        /// <param name="delegateIdLow">Agent PUID high value.</param>
        /// <param name="searchPuidHigh">Customer Puid high value</param>
        /// <param name="searchPuidLow">Customer Puid low value</param>
        /// <param name="roleSetXml">Set roleSet parameter </param>
        /// <param name="fullData">Indicates how much information to return. </param>
        /// <param name="max">The number of results to return. </param>
        /// <param name="errorXml">This parameter is deprecated and no longer used. </param>
        /// <param name="moreRows">Identifies whether the number of matching records exceeds the maximum number of records that can be returned.</param>
        /// <param name="accountInfoCount">The number of accounts returned. </param>
        /// <param name="accountInfoSetXml">The set of accounts found in the search and related account information. </param>
        void GetAccountIdFromPuid(int delegateIdHigh, int delegateIdLow, int searchPuidHigh, int searchPuidLow, string roleSetXml, bool fullData, int max, out string errorXml, out int moreRows, out int accountInfoCount, out string accountInfoSetXml);

        /// <summary>
        /// The API retrieves accounts with a given merchant reference number (MRN). 
        /// </summary>
        /// <param name="delegateIdHigh">Agent PUID high value.</param>
        /// <param name="delegateIdLow">Agent PUID low value.</param>
        /// <param name="billingInfoSearchCriteriaXml">The criteria by which to search.</param>
        /// <param name="billingInfoSetXml">The set of information returned by SearchBillingInfo.</param>
        void SearchBillingInfo(int delegateIdHigh, int delegateIdLow, string billingInfoSearchCriteriaXml, out string billingInfoSetXml);

        /// <summary>
        /// The API retrieves account information such as owner name, shipping address, and phone.
        /// </summary>
        /// <param name="lDelegateIdHigh">The Passport Unique identifier (PUID) of the CSR making the request. If there is no delegate, set this parameter to zero and specify lRequesterIdHigh.</param>
        /// <param name="lDelegateIdLow">The Passport Unique identifier (PUID) of the CSR making the request. If there is no delegate, set this parameter to zero and specify lRequesterIdLow.</param>
        /// <param name="lRequesterIdHigh">The PUID of the customer making the request. For requests that users initiate via partners, the requester must be an account administrator. </param>
        /// <param name="lRequesterIdLow">The PUID of the customer making the request. For requests that users initiate via partners, the requester must be an account administrator. </param>
        /// <param name="bstrAccountId">One of several kinds of identifiers. To retrieve information for all accounts associated with a subscription, payment instrument, or service instance, specify the identifier for the respective object. To retrieve information only for one account, specify the object identifier. This parameter requires a 16-character string; it cannot be null or the empty string (""). </param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used. See SCS Errors for information about error handling. </param>
        /// <param name="pbstrAccountInfoXML">The information used to create the account. All nodes are returned. An empty node is returned for a missing value.</param>
        void GetAccountInfo(int lDelegateIdHigh, int lDelegateIdLow, int lRequesterIdHigh, int lRequesterIdLow, string bstrAccountId, out string pbstrErrorXML, out string pbstrAccountInfoXML);

        /// <summary>
        /// Returns the permits available for the specified object, either at the account, subscription, or service instance level. 
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID of the CSR making the request. If there is no delegate, set this parameter to zero and specify lRequesterIdHigh and lRequesterIdLow.</param>
        /// <param name="lDelegateIdLow">The PUID of the CSR making the request. If there is no delegate, set this parameter to zero and specify lRequesterIdHigh and lRequesterIdLow.</param>
        /// <param name="lRequesterIdHigh">The PUID of the customer making the request. For requests initiated by CSRs, set this value to zero and specify lDelegateIdHigh and lDelegateIdLow. For requests that users initiate via partners, the requester must be an account administrator.</param>
        /// <param name="lRequestedIdLow">The PUID of the customer making the request. For requests initiated by CSRs, set this value to zero and specify lDelegateIdHigh and lDelegateIdLow. For requests that users initiate via partners, the requester must be an account administrator.</param>
        /// <param name="bstrObjectId">The identifier of the account, subscription, or service instance for retrieving permits. This parameter requires a 16-character string; it cannot be null or the empty string ("").</param>
        /// <param name="fDeep">Not supported.</param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used. See SCS Errors for information about error handling.</param>
        /// <param name="plPermitCount">The number of permits returned. </param>
        /// <param name="pbstrPermitSetXML">Permit information returned for the BillableAccountAdmin, SubscriptionAdmin, and SubscriptionViewer roles.</param>
        void GetPermitsForObjectId(int lDelegateIdHigh, int lDelegateIdLow, int lRequesterIdHigh, int lRequestedIdLow, string bstrObjectId, bool fDeep, out string pbstrErrorXML, out int plPermitCount, out string pbstrPermitSetXML);

        /// <summary>
        /// Retrieves information on accounts that have the specified payment instrument criteria. 
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID of the CSR making the request. </param>
        /// <param name="lDelegateIdLow">The PUID of the CSR making the request. </param>
        /// <param name="fFullData">Indicates how much information to return. </param>
        /// <param name="lMax">The number of results to return. The value for this parameter must be less than or equal to 25. </param>
        /// <param name="bstrPaymentInstrumentInfoXML">The fields to search. </param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used.</param>
        /// <param name="pfMoreRows">Identifies whether the number of matching records exceeds the maximum number of records that can be returned. </param>
        /// <param name="plAccountInfoCount">The number of accounts returned. </param>
        /// <param name="pbstrAccountInfoSetXML">The set of accounts found in the search, as well as appropriate related account information. </param>
        void GetAccountIdFromPaymentInstrumentInfo(int lDelegateIdHigh, int lDelegateIdLow, bool fFullData, int lMax, string bstrPaymentInstrumentInfoXML, out string pbstrErrorXML, out int pfMoreRows, out int plAccountInfoCount, out string pbstrAccountInfoSetXML);

        /// <summary>
        /// Retrieves the most recent comments for an account. 
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID of the CSR performing the operation. </param>
        /// <param name="lDelegateIdLow">The PUID of the CSR performing the operation. </param>
        /// <param name="bstrAccountId">The account ID.</param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used.</param>
        /// <param name="plCommentInfoSetCount">The number of comments returned. </param>
        /// <param name="pbstrCommentInfoSetXML">Information comments. </param>
        void GetComments(int lDelegateIdHigh, int lDelegateIdLow, string bstrAccountId, out string pbstrErrorXML, out int plCommentInfoSetCount, out string pbstrCommentInfoSetXML);

        /// <summary>
        /// Adds a comment to an account.
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID of the CSR performing the operation.</param>
        /// <param name="lDelegateIdLow">The PUID of the CSR performing the operation.</param>
        /// <param name="bstrAccountId">The ID of the account.</param>
        /// <param name="bstrCommentInfoXML">The information Commerce Platform uses to log a descriptive comment.</param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used. </param>
        void AddComment(int lDelegateIdHigh, int lDelegateIdLow, string bstrAccountId, string bstrCommentInfoXML, out string pbstrErrorXML);

        /// <summary>
        /// Retrieves information about the subscription or set of subscriptions associated with the corresponding ObjectId 
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID of the CSR making the request.</param>
        /// <param name="lDelegateIdLow">The PUID of the CSR making the request.</param>
        /// <param name="lRequesterIdHigh">The PUID of the customer making the request.</param>
        /// <param name="lRequesterIdLow">The PUID of the customer making the request.</param>
        /// <param name="bstrObjectId">The identifier of a subscription, service instance, payment instrument, or account.</param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used.</param>
        /// <param name="plSubscriptionInfoCount">The number of subscriptions returned by the GetSubscriptions method. </param>
        /// <param name="pbstrSubscriptionInfoSetXML">The information set containing the subscriptions returned by the GetSubscriptions method. </param>
        void GetSubscriptions(int lDelegateIdHigh, int lDelegateIdLow, int lRequesterIdHigh, int lRequesterIdLow, string bstrObjectId, out string pbstrErrorXML, out int plSubscriptionInfoCount, out string pbstrSubscriptionInfoSetXML);

        /// <summary>
        /// Enables a CSR to view the details related to a line item in an offering's billing history.
        /// </summary>
        /// <param name="delegateIdHigh">The PUID. If a delegate is performing this operation on behalf of the requester, then this value should be the PUID of the delegate. If a CSR is operating on behalf of a user, this will be the PUID of the CSR. If there is no delegate, this must be zero, in which case the requester must be specified.</param>
        /// <param name="delegateIdLow">The PUID. If a delegate is performing this operation on behalf of the requester, then this value should be the PUID of the delegate. If a CSR is operating on behalf of a user, this will be the PUID of the CSR. If there is no delegate, this must be zero, in which case the requester must be specified.</param>
        /// <param name="lineItemId">The ID of the item for which to view the linked line items. This parameter cannot be null or the empty string (""). You can get the line item ID by calling the GetStatement Method, which returns the ID as the idRef attribute of the XML schema.</param>
        /// <param name="returnHistory">Indicates whether to return the line item history (True) or not (False). If True, the lineItemHistorySetXML output parameter will contain data. Required.
        ///                             This parameter cannot be null or the empty string (""). For C or C++, pass VARIANT_TRUE or VARIANT_FALSE; for script code, pass TRUE or FALSE.</param>
        /// <param name="errorXML">This parameter is deprecated and no longer used. See SCS Errors for information about error handling.</param>
        /// <param name="lineItemHistorySetXML">The set of line items that are linked together.</param>
        /// <param name="mcv">The MCV value (maximum creditable or chargeable value) for the line item.</param>
        void GetLineItemHistory(int delegateIdHigh, int delegateIdLow, string lineItemId, bool returnHistory, out string errorXML, out string lineItemHistorySetXML, out string mcv);

        /// <summary>
        /// Offsets a charge or credit item. 
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID high of the CSR making the request. </param>
        /// <param name="lDelegateIdLow">The PUID low of the CSR making the request. </param>
        /// <param name="bstrTrackingGUID">The caller-generated GUID that identifies the transaction.</param>
        /// <param name="bstrLineItemId">The identifier of the item to offset. </param>
        /// <param name="lFinancialReportingCode">The credit code to apply. </param>
        /// <param name="fImmediatelySettle">Not currently supported and must be set to false. </param>
        /// <param name="bstrAmount">The line item amount to offset.</param>
        /// <param name="bstrCommentInfoXML">The information SCS uses to log a descriptive comment.</param>
        /// <param name="bstrReservedXML">Reserved for future use, defaults to NULL.</param>
        /// <param name="pbstrError">This parameter is deprecated and no longer used.</param>
        /// <param name="pbstrNewLineItemXML">The identifier code of the new line item.</param>
        void OffsetLineItem2(int lDelegateIdHigh, int lDelegateIdLow, string bstrTrackingGUID, string bstrLineItemId, int lFinancialReportingCode, bool fImmediatelySettle, string bstrAmount, string bstrCommentInfoXML, string bstrReservedXML, out string pbstrError, out string pbstrNewLineItemXML);

        /// <summary>
        /// Authorizes and immediately settles an outstanding balance on a payment instrument.
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID high of the CSR or delegate performing the operation.</param>
        /// <param name="lDelegateIdLow">The PUID low of the CSR or delegate performing the operation.</param>
        /// <param name="bstrTrackingGUID">The caller-generated GUID that identifies the transaction.</param>
        /// <param name="bstrPaymentInstrumentId">The payment instrument identifier whose balance has to be settled.</param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used.</param>
        /// <param name="pbstrAmountChargedXML">The monetary amount that was immediately charged to the payment instrument specified.</param>
        void SettleBalance(int lDelegateIdHigh, int lDelegateIdLow, string bstrTrackingGUID, string bstrPaymentInstrumentId, out string pbstrErrorXML, out string pbstrAmountChargedXML);

        /// <summary>
        /// Retrieves the billing activity on a given account or payment instrument in a specified period.
        /// </summary>
        /// <param name="delegateIdHigh">The Passport Unique identifier (PUID) of the delegate performing the operation on behalf of the user.</param>
        /// <param name="delegateIdLow">The Passport Unique identifier (PUID) of the delegate performing the operation on behalf of the user.</param>
        /// <param name="requesterIdHigh">The PUID for the account owner for whom the account information is returned.</param>
        /// <param name="requesterIdLow">The PUID for the account owner for whom the account information is returned.</param>
        /// <param name="objectId">The identifier of the account, address, or payment instrument for which information is being requested.</param>
        /// <param name="beginBillingPeriodId">The identifier of the first billing period for which to retrieve account activity information.</param>
        /// <param name="endBillingPeriodId">The identifier of the last billing period for which to retrieve account activity information.</param>
        /// <param name="returnStatementSet">This flag indicates whether the information in <AccountStatementInfoSet> should be returned and what level (all or a subset) of the data should be returned. Required.</param>
        /// <param name="returnNotificationSet">Indicates whether notifications should be returned (True) or not (False).</param>
        /// <param name="orderId">The identifier of the order to retrieve info. </param>
        /// <param name="errorXml">This parameter is deprecated and no longer used. See SCS Errors for information about error handling.</param>
        /// <param name="accountStatementInfoSetXml">Retrieved information for the accounts or payment instrument, or order.</param>
        /// <param name="userNotificationSetXml">The notifications or status values retrieved for the subscriptions or payment instruments specified.</param>
        void GetStatementEx(int delegateIdHigh, int delegateIdLow, int requesterIdHigh, int requesterIdLow, string objectId, uint beginBillingPeriodId, uint endBillingPeriodId, byte returnStatementSet, bool returnNotificationSet, string orderId, out string errorXml, out string accountStatementInfoSetXml, out string userNotificationSetXml);

        /// <summary>
        /// Retrieves one or all of the payment instruments for a specified account.
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID of the CSR making the request.</param>
        /// <param name="lDelegateIdLow">The PUID of the CSR making the request.</param>
        /// <param name="lRequesterIdHigh">Identifier of a payment instrument or account.</param>
        /// <param name="lRequesterIdLow">Identifier of a payment instrument or account.</param>
        /// <param name="bstrObjectId">Identifier of a payment instrument or account.</param>
        /// <param name="fReturnRemoved">Specifies whether payment instruments in the removed status are returned.</param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used.</param>
        /// <param name="plPaymentInstrumentInfoCount">The number of payment instruments returned.</param>
        /// <param name="pbstrPaymentInstrumentInfoSetXML">The payment instrument information.</param>
        void GetPaymentInstrumentsEx(int lDelegateIdHigh, int lDelegateIdLow, int lRequesterIdHigh, int lRequesterIdLow, string bstrObjectId, bool fReturnRemoved, out string pbstrErrorXML, out int plPaymentInstrumentInfoCount, out string pbstrPaymentInstrumentInfoSetXML);

        /// <summary>
        /// Deletes a payment instrument from an account.
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID of the CSR making the request.</param>
        /// <param name="lDelegateIdLow">The PUID of the CSR making the request.</param>
        /// <param name="lRequesterIdHigh">The PUID of the customer making the request.</param>
        /// <param name="lRequesterIdLow">The PUID of the customer making the request.</param>
        /// <param name="bstrPaymentInstrumentId">The payment instrument identifier to remove.</param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used.</param>
        void RemovePaymentInstrument(int lDelegateIdHigh, int lDelegateIdLow, int lRequesterIdHigh, int lRequesterIdLow, string bstrPaymentInstrumentId, out string pbstrErrorXML);

        /// <summary>
        /// This method extends a prepaid or token-based subscription by a specified number of days.
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID high of the CSR performing the operation.</param>
        /// <param name="lDelegateIdLow">The PUID low of the CSR performing the operation.</param>
        /// <param name="bstrTrackingGUID">The caller-generated GUID that identifies the transaction and allows the partner to re-query the billing system for the results of this transaction. </param>
        /// <param name="bstrSubscriptionId">The subscription identifier for which adjustment information is being requested.</param>
        /// <param name="lNumberOfDays">The number of days that the subscription should be extended by (positive or negative). </param>
        /// <param name="bstrCommentInfoXML">The information SCS uses to log a descriptive comment.</param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used.</param>
        /// <param name="pbstrNewExpirationDate">The new expiration date of the subscription.</param>
        void ExtendSubscription(int lDelegateIdHigh, int lDelegateIdLow, string bstrTrackingGUID, string bstrSubscriptionId, int lNumberOfDays, string bstrCommentInfoXML, out string pbstrErrorXML, out string pbstrNewExpirationDate);

        /// <summary>
        /// Retrieves the offerings available for an existing customer based on the country, currency, and locale information for the account.
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID high of the CSR making the request.</param>
        /// <param name="lDelegateIdLow">The PUID low of the CSR making the request.</param>
        /// <param name="lRequesterIdHigh">The PUID high of the customer making the request.</param>
        /// <param name="lRequesterIdLow">The PUID low of the customer making the request.</param>
        /// <param name="bstrAccountId">The account identifier to search.</param>
        /// <param name="bstrOfferingGUID">The offering identifier of one of the subscriptions owned by the account.</param>
        /// <param name="bstrCategory">A unique string representing a partner-configured category.</param>
        /// <param name="bstrFilter">The type of offering to return: Base, Upgrade, Downgrade, Renewal, or an empty string (to return all upgrade, downgrade, and renewal relations).</param>
        /// <param name="bstrTokenId">Optional. The identifier of the token instance or discount coupon code that the user intends to use while purchasing an offering. </param>
        /// <param name="pbstrErrorText">The results XML for error. </param>
        /// <param name="plOfferingInfoCount">The number of offerings returned.</param>
        /// <param name="pbstrOfferingInfoSetXML">The requested offering information.</param>
        void GetEligibleOfferingsEx(int lDelegateIdHigh, int lDelegateIdLow, int lRequesterIdHigh, int lRequesterIdLow, string bstrAccountId, string bstrOfferingGUID, string bstrCategory, string bstrFilter, string bstrTokenId, out string pbstrErrorText, out int plOfferingInfoCount, out string pbstrOfferingInfoSetXML);

        /// <summary>
        /// Returns the information about history of the subscription, including the offer containing the subscription.
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID high of the CSR making the request.</param>
        /// <param name="lDelegateIdLow">The PUID low of the CSR making the request.</param>
        /// <param name="lRequesterIdHigh">The PUID high of the customer making the request.</param>
        /// <param name="lRequesterIdLow">The PUID low of the customer making the request.</param>
        /// <param name="bstrSubscriptionId">The identifier of the subscription whose history is to be retrieved.</param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used. See SCS Errors for information about error handling.</param>
        /// <param name="plHistoryEventCount">The number of history events returned. </param>
        /// <param name="pbstrSubscriptionHistoryEventSetXML">The history of the subscription.</param>
        void GetSubscriptionHistory(int lDelegateIdHigh, int lDelegateIdLow, int lRequesterIdHigh, int lRequesterIdLow, string bstrSubscriptionId, out string pbstrErrorXML, out int plHistoryEventCount, out string pbstrSubscriptionHistoryEventSetXML);

        /// <summary>
        /// Closes open payment instrument line item balances by changing their status from Open to Ready for positive balances and Open to RefundNow for negative balances. 
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID of the CSR making the request.</param>
        /// <param name="lDelegateIdLow">The PUID of the CSR making the request.</param>
        /// <param name="bstrTrackingGUID">The caller-generated GUID that identifies the transaction.</param>
        /// <param name="bstrObjectId">The payment instrument identifier.</param>
        /// <param name="bstrLineItemId">The identifier of the item to close, if any.</param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used.</param>
        /// <param name="pbstrAmountChargedXML">The monetary amount moved from Open to Ready for the specified payment instrument.</param>
        void CloseBalance(int lDelegateIdHigh, int lDelegateIdLow, string bstrTrackingGUID, string bstrObjectId, string bstrLineItemId, out string pbstrErrorXML, out string pbstrAmountChargedXML);

        /// <summary>
        /// Changes the state of a subscription to either the EXPIRED or CANCELED state, depending on the value for the grace period, and closes the balance for the subscription's payment instrument.
        /// </summary>
        /// <param name="delegatePUIDHigh">The PUID high of the CSR making the request.</param>
        /// <param name="delegatePUIDLow">The PUID low of the CSR making the request.</param>
        /// <param name="requesterPUIDHigh">The PUID high of the user making the request.</param>
        /// <param name="requesterPUIDLow">The PUID low of the user making the request.</param>
        /// <param name="trackingGUID">The caller-generated GUID that identifies the transaction.</param>
        /// <param name="computeOnly">The option to have Commerce Platform validate parameters and calculate the results of the transaction without committing anything to the database.</param>
        /// <param name="subscriptionId">The identifier of the subscription whose status changed.</param>
        /// <param name="cancelDate">Indicates how and when to cancel the subscription. There are four acceptable values: DELAYED_EXPIRE, UNDO_DELAYED_EXPIRE, IMMEDIATE_EXPIRE, IMMEDIATE_CANCEL.</param>
        /// <param name="commentInfoXML">The information Commerce Platform uses to log a descriptive comment.</param>
        /// <param name="closeBalance">If closeBalance is equal to FALSE, does not close balance at end.</param>
        /// <param name="amountChargedXML">Information about the cancellation fees (if any) assessed for early cancellation.</param>
        /// <param name="subscriptionStatusInfoXML">The status of the subscription.</param>
        /// <param name="removedServiceInstanceCount">The number of service instances removed as a result of the cancellation. </param>
        /// <param name="removedServiceInstanceSetXML">The removed service instances.</param>
        void CancelSubscriptionEx(int delegatePUIDHigh, int delegatePUIDLow, int requesterPUIDHigh, int requesterPUIDLow, string trackingGUID, bool computeOnly, string subscriptionId, string cancelDate, string commentInfoXML, bool closeBalance, out string amountChargedXML, out string subscriptionStatusInfoXML, out int removedServiceInstanceCount, out string removedServiceInstanceSetXML);

        /// <summary>
        /// Changes the subscription status from expired to enabled.
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID high of the partner making the request.</param>
        /// <param name="lDelegateIdLow">The PUID low of the partner making the request.</param>
        /// <param name="lRequesterIdHigh">The PUID high of the customer making the request. </param>
        /// <param name="lRequesterIdLow">The PUID low of the customer making the request. </param>
        /// <param name="bstrSubscriptionId">The ID of the subscription to reinstate.</param>
        /// <param name="lReserved">Reserved.</param>
        /// <param name="bstrCommentInfoXML">The information SCS uses to log a descriptive comment.</param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used.</param>
        /// <param name="pbstrAmountChargedXML">Returns any charges incurred in the reinstate process. </param>
        /// <param name="pbstrSubscriptionStatusInfoXML">The subscription status and any violations that may be associated with the subscription.</param>
        void ReinstateSubscription(int lDelegateIdHigh, int lDelegateIdLow, int lRequesterIdHigh, int lRequesterIdLow, string bstrSubscriptionId, int lReserved, string bstrCommentInfoXML, out string pbstrErrorXML, out string pbstrAmountChargedXML, out string pbstrSubscriptionStatusInfoXML);

        /// <summary>
        /// Renews a subscription or converts it from one offering to another, adding and removing the appropriate service instances and assessing any applicable conversion fees.
        /// </summary>
        /// <param name="lDelegateIdHigh">The Passport Unique Identifier (PUID) high of the person making the request.</param>
        /// <param name="lDelegateIdLow">The Passport Unique Identifier (PUID) low of the person making the request.</param>
        /// <param name="lRequesterIdHigh">The PUID high of the customer making the request.</param>
        /// <param name="lRequesterIdLow">The PUID low of the customer making the request.</param>
        /// <param name="bstrTrackingGUID">The caller-generated GUID that identifies the transaction.</param>
        /// <param name="fComputeOnly">An option to have SCS validate parameters and calculate the results of the transaction without committing anything to the database. </param>
        /// <param name="lConvertMode">The mode to convert in: modeConvert (0) or modeRenew (1).</param>
        /// <param name="lOverrideFlags">Any override flags for term commits.</param>
        /// <param name="bstrSubscriptionName">The new subscription's friendly name.</param>
        /// <param name="bstrSubscriptionId">The identifier of the currently owned subscription to be converted.</param>
        /// <param name="bstrOfferingGUID">The unique identifier for the offer to which the subscription is converted or renewed.</param>
        /// <param name="lOverrideAmount">The override amount.</param>
        /// <param name="bstrSubscriptionEndDate">The subscription end date.</param>
        /// <param name="bstrReferralSetXML">The referral identifiers associated with this sale.</param>
        /// <param name="bstrPaymentInstrumentId">The payment instrument identifier of the subscription from which you are converting.</param>
        /// <param name="bstrTokenId">Optional. The identifier of the token instance that is used to filter the list of offers eligible for conversion.</param>
        /// <param name="bstrPolicyGUID">Identifier of the policy text.</param>
        /// <param name="lPolicyVersion">The version of the policy signed.</param>
        /// <param name="bstrSignatureDateTime">The date and time the signature was accepted.</param>
        /// <param name="bstrRedirectInputInfoXML">Used to obtain input parameters to start a redirect transaction. Before starting redirect, SCG must know this information.</param>
        /// <param name="bstrDiscountGuid">The identifier of the discount returned in the <DiscountInfoSet> XML Schema by the previous call to either the GetBaseOfferingsEx or GetEligibleOfferingsEx method.</param>
        /// <param name="bstrExtraInfoXML">Contains the payer authentication code.</param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used.</param>
        /// <param name="pbstrAmountChargedXML">The monetary amount that was immediately charged to the payment instrument specified.</param>
        /// <param name="plAddedServiceInstanceSetCount">The number of service components added as a result of the purchase.</param>
        /// <param name="pbstrAddedServiceInstanceSet">The service instances added as a result of the conversion.</param>
        /// <param name="plRemovedServiceInstanceSetCount">The number of service components removed as a result of the conversion.</param>
        /// <param name="pbstrRemoveServiceInstanceSet">The service instances removed as a result of this conversion.</param>
        /// <param name="pbstrRedirectOutputInfoXML">The top-level element. Required.</param>
        void ConvertSubscriptionEx3(
                    int lDelegateIdHigh,
                    int lDelegateIdLow,
                    int lRequesterIdHigh,
                    int lRequesterIdLow,
                    string bstrTrackingGUID,
                    bool fComputeOnly,
                    int lConvertMode,
                    int lOverrideFlags,
                    string bstrSubscriptionName,
                    string bstrSubscriptionId,
                    string bstrOfferingGUID,
                    int lOverrideAmount,
                    string bstrSubscriptionEndDate,
                    string bstrReferralSetXML,
                    string bstrPaymentInstrumentId,
                    string bstrTokenId,
                    string bstrPolicyGUID,
                    int lPolicyVersion,
                    string bstrSignatureDateTime,
                    string bstrRedirectInputInfoXML,
                    string bstrDiscountGuid,
                    string bstrExtraInfoXML,
                    out string pbstrErrorXML,
                    out string pbstrAmountChargedXML,
                    out int plAddedServiceInstanceSetCount,
                    out string pbstrAddedServiceInstanceSet,
                    out int plRemovedServiceInstanceSetCount,
                    out string pbstrRemoveServiceInstanceSet,
                    out string pbstrRedirectOutputInfoXML);

        /// <summary>
        /// Adds a new payment instrument to the specified account.
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID of the CSR making the request.</param>
        /// <param name="lDelegateIdLow">The PUID of the CSR making the request.</param>
        /// <param name="lRequesterIdHigh">The PUID high of the customer making the request.</param>
        /// <param name="lRequesterIdLow">The PUID high of the customer making the request.</param>
        /// <param name="bstrTrackingGUID">The caller-generated GUID that identifies the transaction.</param>
        /// <param name="bstrAccountId">The account identifier for the new payment instrument.</param>
        /// <param name="bstrPaymentInstrumentInfoXML">Information on the new payment instrument.</param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used.</param>
        /// <param name="pbstrPaymentInstrumentId">The unique identifier for the payment instrument.</param>
        /// <param name="pbstrRequiredPaperWorkURL">The URL that uniquely identifies the address for sending paperwork required to complete the authorization of a tax exemption.</param>
        void AddPaymentInstrument(int lDelegateIdHigh, int lDelegateIdLow, int lRequesterIdHigh, int lRequesterIdLow, string bstrTrackingGUID, string bstrAccountId, string bstrPaymentInstrumentInfoXML, out string pbstrErrorXML, out string pbstrPaymentInstrumentId, out string pbstrRequiredPaperWorkURL);

        /// <summary>
        /// Transfers outstanding balances and subscriptions from one payment instrument to another.
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID of the CSR making the request.</param>
        /// <param name="lDelegateIdLow">The PUID of the CSR making the request.</param>
        /// <param name="lRequesterIdHigh">The PUID of the customer making the request. </param>
        /// <param name="lRequesterIdLow">The PUID of the customer making the request. </param>
        /// <param name="bstrTrackingGUID">The caller-generated GUID that identifies the transaction.</param>
        /// <param name="bstrFromPaymentInstrumentId">The identifier of the current payment instrument for the balance or subscriptions.</param>
        /// <param name="bstrToPaymentInstrumentId">The identifier of the new payment instrument for the balance or subscriptions.</param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used.</param>
        /// <param name="pbstrAmountChargedXML">The due balance that was charged immediately to the new payment instrument.</param>
        void SwitchPaymentInstruments(int lDelegateIdHigh, int lDelegateIdLow, int lRequesterIdHigh, int lRequesterIdLow, string bstrTrackingGUID, string bstrFromPaymentInstrumentId, string bstrToPaymentInstrumentId, out string pbstrErrorXML, out string pbstrAmountChargedXML);

        /// <summary>
        /// Retrieves the offerings available for a new customer to purchase, based on the customer's country, currency, language, and other criteria. This method must be called before calling the CreateAccount method to ensure that the token is valid.
        /// </summary>
        /// <param name="bstrOfferingGUID">Optional. The specific offering identifier to retrieve.</param>
        /// <param name="bstrCategory">A unique string representing a category configured by the Offer Modeling team.</param>
        /// <param name="bstrCountry">The two-letter ISO code for the country where the customer is located. </param>
        /// <param name="bstrLocale">Required. The customer's locale code. For a list of valid codes, see Locale Codes.</param>
        /// <param name="bstrCurrency">Required. The three-letter ISO code for the customer's purchase currency.</param>
        /// <param name="bstrTokenId">Optional. The identifier of the token instance or discount coupon code that the user intends to use while purchasing an offering.</param>
        /// <param name="pbstrErrorText">The results XML. If there is a failure, the Soap Toolkit's SoapServer object returns the result to the SoapClient object.</param>
        /// <param name="plOfferingInfoCount">The number of offerings returned.</param>
        /// <param name="pbstrOfferingInfoSetXML">The requested offering information.</param>
        void GetBaseOfferingsEx(string bstrOfferingGUID, string bstrCategory, string bstrCountry, string bstrLocale, string bstrCurrency, string bstrTokenId, out string pbstrErrorText, out int plOfferingInfoCount, out string pbstrOfferingInfoSetXML);

        /// <summary>
        /// Credits an arbitrary amount to a payment instrument.
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID the CSR making the request.</param>
        /// <param name="lDelegateIdLow">The PUID the CSR making the request.</param>
        /// <param name="bstrTrackingGUID">The caller-generated GUID that identifies the transaction.</param>
        /// <param name="bstrPaymentInstrumentId">The payment instrument identifier to credit.</param>
        /// <param name="bstrSkuReferenceXML">Sku Reference information.</param>
        /// <param name="bstrPropertyXML">Property information.</param>
        /// <param name="lFinancialReportingCode">The reason the payment instrument was credited.</param>
        /// <param name="bstrAmount">The amount to credit or charge in the account's currency.</param>
        /// <param name="currency">The ISO 3-letter currency code representing the customer's preferred currency.</param>
        /// <param name="fImmediatelySettle">An option to settle immediately.</param>
        /// <param name="bstrCommentInfoXML">The information SCS uses to log a descriptive comment.</param>
        /// <param name="pbstrError">This parameter is deprecated and no longer used.</param>
        void CreditPaymentInstrumentEx3(int lDelegateIdHigh, int lDelegateIdLow, string bstrTrackingGUID, string bstrPaymentInstrumentId, string bstrSkuReferenceXML, string bstrPropertyXML, int lFinancialReportingCode, string bstrAmount, string currency, bool fImmediatelySettle, string bstrCommentInfoXML, string bstrStoredValueLotExpirationDate, string bstrStoredValueLotType, string bstrStoredValueSku, out string pbstrError);

        /// <summary>
        /// Updates information for the specified payment instrument.
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID the CSR making the request.</param>
        /// <param name="lDelegateIdLow">The PUID the CSR making the request.</param>
        /// <param name="lRequesterIdHigh">The PUID of the customer making the request.</param>
        /// <param name="lRequesterIdLow">The PUID of the customer making the request.</param>
        /// <param name="bstrPaymentInstrumentId">The identifier of the payment instrument to update.</param>
        /// <param name="bstrPaymentInstrumentInfoXML">The payment information to be updated.</param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used.</param>
        void UpdatePaymentInstrumentInfo(int lDelegateIdHigh, int lDelegateIdLow, int lRequesterIdHigh, int lRequesterIdLow, string bstrPaymentInstrumentId, string bstrPaymentInstrumentInfoXML, out string pbstrErrorXML);

        /// <summary>
        /// Removes any roles assigned to a service instance, deprovisions the service instance, and updates authorization data in UPS. To prevent misuse, this method should be used with caution and carefully tested in integration with the service partner.
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID of the CSR making the request.</param>
        /// <param name="lDelegateIdLow">The PUID of the CSR making the request.</param>
        /// <param name="lRequesterIdHigh">The customer making the request. </param>
        /// <param name="lRequesterIdLow">The customer making the request. </param>
        /// <param name="bstrServiceInstanceSetXML">XML structure containing the service instances to deprovision.</param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used.</param>
        void DeprovisionServices(int lDelegateIdHigh, int lDelegateIdLow, int lRequesterIdHigh, int lRequesterIdLow, string bstrServiceInstanceSetXML, out string pbstrErrorXML);

        /// <summary>
        /// Updates an account's subscription information. Updates can be applied to enabled, suspended, or expired subscriptions. UpdateSubscriptionInfo can no longer be used to add referral data to a subscription.
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID high of the CSR making the request.</param>
        /// <param name="lDelegateIdLow">The PUID low of the CSR making the request.</param>
        /// <param name="lRequesterIdHigh">The PUID high of the customer making the request. </param>
        /// <param name="lRequesterIdLow">The PUID low of the customer making the request. </param>
        /// <param name="bstrTrackingGUID">The caller-generated GUID that identifies the transaction.</param>
        /// <param name="bstrSubscriptionId">The identifier of the subscription to update. </param>
        /// <param name="bstrSubscriptionInfoXML">The subscription information to update.</param>
        /// <param name="bstrReferralSetXML">Not used. </param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used. </param>
        void UpdateSubscriptionInfo(int lDelegateIdHigh, int lDelegateIdLow, int lRequesterIdHigh, int lRequesterIdLow, string bstrTrackingGUID, string bstrSubscriptionId, string bstrSubscriptionInfoXML, string bstrReferralSetXML, out string pbstrErrorXML);

        /// <summary>
        /// Adds a violation to an account or subscription. 
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID high of the CSR making the request. </param>
        /// <param name="lDelegateIdLow">The PUID low of the CSR making the request. </param>
        /// <param name="bstrObjectId">The account ID, subscription ID, or item instance ID affected by the violation. </param>
        /// <param name="lViolationId">The ID of the violation to add to the account or the subscription.</param>
        /// <param name="bstrCommentInfoXML">The XML structure containing information SCS uses to log a descriptive comment.</param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used.</param>
        /// <param name="pbstrStatusInfoXML">The XML structure that describes the status of the subscription or account at the end of the call. </param>
        /// <param name="pbstrAmountChargedXML">The XML structure that describes the monetary amount charged because of a status change.</param>
        void AddViolation(int lDelegateIdHigh, int lDelegateIdLow, string bstrObjectId, int lViolationId, string bstrCommentInfoXML, out string pbstrErrorXML, out string pbstrStatusInfoXML, out string pbstrAmountChargedXML);

        /// <summary>
        /// Removes a violation from an account or subscription.
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID high of the CSR making the request. </param>
        /// <param name="lDelegateIdLow">The PUID low of the CSR making the request. </param>
        /// <param name="bstrObjectId">The account id, subscription id, or item instance id for which the violation should be removed.</param>
        /// <param name="lViolationId">The violation to remove from the account or the subscription.</param>
        /// <param name="bstrCommentInfoXML">The information SCS uses to log a descriptive comment.</param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used.</param>
        /// <param name="pbstrStatusInfoXML">The status of the subscription or account at the end of the call.</param>
        /// <param name="pbstrAmountChargedXML">The monetary amount charged as a result of any status change.</param>
        void RemoveViolation(int lDelegateIdHigh, int lDelegateIdLow, string bstrObjectId, int lViolationId, string bstrCommentInfoXML, out string pbstrErrorXML, out string pbstrStatusInfoXML, out string pbstrAmountChargedXML);

        /// <summary>
        /// This read-only method calls a third-party Delivery Address Verification (DAV) service to perform address validation.
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID high of the CSR making the request.</param>
        /// <param name="lDelegateIdLow">The PUID low of the CSR making the request.</param>
        /// <param name="referenceTrackingGuid">Tracking guid.</param>
        /// <param name="addressInfoXML">The input address that is mapped against the Delivery Address Verification Service.</param>
        /// <param name="MapAddressInfoSet">With the current implementation, one standardized address (<MapAddressInfoSet>) is returned whenever there is no error.</param>
        void MapAddress(int delegateIdHigh, int delegateIdLow, string referenceTrackingGuid, string addressInfoXML, out string MapAddressInfoSet);

        /// <summary>
        /// Changes the account administration or adds additional delegated administrators to an account.
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID high of the CSR making the request.</param>
        /// <param name="lDelegateIdLow">The PUID low of the CSR making the request.</param>
        /// <param name="lRequesterIdHigh">The PUID high of the customer making the request.</param>
        /// <param name="lRequesterIdLow">The PUID low of the customer making the request.</param>
        /// <param name="bstrObjectId">The identifier of the account or subscription for the new role assignment.</param>
        /// <param name="bstrRoleAssignmentXML">Information about the new administrator or viewer role.</param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used.</param>
        void AddRoleAssignment(int lDelegateIdHigh, int lDelegateIdLow, int lRequesterIdHigh, int lRequesterIdLow, string bstrObjectId, string bstrRoleAssignmentXML, out string pbstrErrorXML);

        /// <summary>
        /// Removes roles assigned to the account or subscription. For information on the roles for each object type.
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID high of the CSR making the request.</param>
        /// <param name="lDelegateIdLow">The PUID low of the CSR making the request.</param>
        /// <param name="lRequesterIdHigh">The PUID high of the person removing the role.</param>
        /// <param name="lRequesterIdLow">The PUID low of the person removing the role.</param>
        /// <param name="bstrObjectId">The ID of the account or subscription from which to remove the role assignments.</param>
        /// <param name="bstrRoleAssignmentXML">The role assignment to remove. </param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used. </param>
        void RemoveRoleAssignment(int lDelegateIdHigh, int lDelegateIdLow, int lRequesterIdHigh, int lRequesterIdLow, string bstrObjectId, string bstrRoleAssignmentXML, out string pbstrErrorXML);

        /// <summary>
        /// Transfers outstanding balances from the current payment instrument to a new payment instrument.
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID high of the CSR making the request.</param>
        /// <param name="lDelegateIdLow">The PUID low of the CSR making the request.</param>
        /// <param name="bstrTrackingGUID">The caller-generated GUID that identifies the transaction.</param>
        /// <param name="bstrFromPaymentInstrumentId">The identifier of the source payment instrument for the balance.</param>
        /// <param name="bstrToPaymentInstrumentId">The identifier of the destination payment instrument for the balance.</param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used.</param>
        /// <param name="pbstrAmountChargedXML">The due balance that was charged immediately to the new payment instrument.</param>
        void TransferBalance(int lDelegateIdHigh, int lDelegateIdLow, string bstrTrackingGUID, string bstrFromPaymentInstrumentId, string bstrToPaymentInstrumentId, out string pbstrErrorXML, out string pbstrAmountChargedXML);

        /// <summary>
        /// Retrieves available and previously applied adjustments for a subscription.
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID of the CSR performing the operation. </param>
        /// <param name="lDelegateIdLow">The PUID of the CSR performing the operation. </param>
        /// <param name="bstrSubscriptionId">The subscription ID to query.</param>
        /// <param name="lBillingPeriodId">The billing period to query.</param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used.</param>
        /// <param name="pcAppliedAdjustmentCount">The number of applied adjustments returned in pbstrAppliedAdjustmentSetXML.</param>
        /// <param name="pbstrAppliedAdjustmentSetXML">The applied adjustment information retrieved for the specified subscription and billing period.</param>
        /// <param name="pcAvailableAdjustmentCount">The number of available adjustments returned in pbstrAvailableAdjustmentSetXML. </param>
        /// <param name="pbstrAvailableAdjustmentSetXML">The available adjustment information retrieved for the specified subscription and billing period.</param>
        void GetAdjustments(int lDelegateIdHigh, int lDelegateIdLow, string bstrSubscriptionId, int lBillingPeriodId, out string pbstrErrorXML, out int pcAppliedAdjustmentCount, out string pbstrAppliedAdjustmentSetXML, out int pcAvailableAdjustmentCount, out string pbstrAvailableAdjustmentSetXML);

        /// <summary>
        /// Applies an available adjustment to a particular subscription. 
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID of the CSR performing the operation.</param>
        /// <param name="lDelegateIdLow">The PUID of the CSR performing the operation.</param>
        /// <param name="bstrSubscriptionId">The subscription ID for the adjustment.</param>
        /// <param name="lBillingPeriodId">The billing period affected by the adjustment.</param>
        /// <param name="bstrAdjustmentGUID">The ID of the adjustment to apply. </param>
        /// <param name="bstrAmount">The amount to apply to the adjustment.</param>
        /// <param name="bstrCommentTextXML">The information SCS uses to log a descriptive comment.</param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used.</param>
        void AdjustSubscription(int lDelegateIdHigh, int lDelegateIdLow, string bstrSubscriptionId, int lBillingPeriodId, string bstrAdjustmentGUID, string bstrAmount, string bstrCommentTextXML, out string pbstrErrorXML);

        /// <summary>
        /// This method blacklists a token so that it can no longer be used.
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID high of the CSR making the request.</param>
        /// <param name="lDelegateIdLow">The PUID low of the CSR making the request.</param>
        /// <param name="bstrTrackingGUID">The caller-generated GUID that identifies the transaction.</param>
        /// <param name="bstrToken">The token identifier or sequence number, depending on TokenType.</param>
        /// <param name="lTokenType">The type of token. If 1, Token is a token identifier; if 0, Token is a token sequence number.</param>
        /// <param name="bstrEffectiveDateTime">The date and time when blacklisting becomes effective.</param>
        /// <param name="bstrSubscriptionAction">The action to take on the subscription associated with the token.</param>
        /// <param name="bstrReason">The reason for blacklisting the token.</param>
        /// <param name="bstrDescription">A description of the blacklisting, required when Reason is OTHER.</param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used.</param>
        /// <param name="pbstrBlacklistActionSetXML">XML encapsulating information about the blacklisting.</param>
        /// <param name="plBlacklistActionSetCount">The number of actions.</param>
        void BlacklistToken(int lDelegateIdHigh, int lDelegateIdLow, string bstrTrackingGUID, string bstrToken, int lTokenType, string bstrEffectiveDateTime, string bstrSubscriptionAction, string bstrReason, string bstrDescription, out string pbstrErrorXML, out string pbstrBlacklistActionSetXML, out int plBlacklistActionSetCount);

        /// <summary>
        /// Checks whether a payment instrument is on the banned payment instrument list.
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID high of the CSR making the request.</param>
        /// <param name="lDelegateIdLow">The PUID low of the CSR making the request.</param>
        /// <param name="bstrPaymentInstrumentInfoXML">The ban information on payment instrument.</param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used.</param>
        /// <param name="pIIsBanned">Identifies whether the payment instrument is banned.
        /// 0: Payment instrument is not banned.
        /// 1: Payment instrument is banned. 
        /// </param>
        void IsPaymentInstrumentBanned(int lDelegateIdHigh, int lDelegateIdLow, string bstrPaymentInstrumentInfoXML, out string pbstrErrorXML, out int pIIsBanned);

        /// <summary>
        /// Adds a specific credit or debit card number or Qwest BTN to the list of banned payment instruments.
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID high of the CSR making the request.</param>
        /// <param name="lDelegateIdLow">The PUID low of the CSR making the request.</param>
        /// <param name="lReasonCode">The reason the payment instrument was banned.</param>
        /// <param name="bstrPaymentInstrumentInfoXML">Information that selects the payment instrument to ban.</param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used.</param>
        void BanPaymentInstrument(int lDelegateIdHigh, int lDelegateIdLow, int lReasonCode, string bstrPaymentInstrumentInfoXML, out string pbstrErrorXML);

        /// <summary>
        /// Removes a payment instrument from the banned list.
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID high of the CSR making the request.</param>
        /// <param name="lDelegateIdLow">The PUID low of the CSR making the request.</param>
        /// <param name="bstrPaymentInstrumentInfoXML">The XML structure containing information for banning the payment instrument.</param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used.</param>
        void UnbanPaymentInstrument(int lDelegateIdHigh, int lDelegateIdLow, string bstrPaymentInstrumentInfoXML, out string pbstrErrorXML);

        /// <summary>
        /// Allows a user to check the status and details of the specified token. 
        /// </summary>
        /// <param name="lDelegateIdHigh">The PUID high of the CSR making the request.</param>
        /// <param name="lDelegateIdLow">The PUID low of the CSR making the request.</param>
        /// <param name="bstrToken">The token ID or sequence number, depending on lTokenType.</param>
        /// <param name="lTokenType">The type of token.
        ///If assigned 1, bstrToken is a token ID;
        ///if assigned 0, bstrToken is a token sequence number. </param>
        /// <param name="pbstrErrorXML">This parameter is deprecated and no longer used.</param>
        /// <param name="pbstrTokenOrderXML">XML Schema containing details about the token.</param>
        void GetTokenInfoEx(int lDelegateIdHigh, int lDelegateIdLow, string bstrToken, int lTokenType, out string pbstrErrorXML, out string pbstrTokenOrderXML);

        /// <summary>
        /// Returns a replacement personal identification number (PIN) to a customer service representative (CSR).
        /// </summary>
        /// <param name="delegatePuidHigh">The PUID of the CSR performing the operation.</param>
        /// <param name="delegatePuidLow">The PUID of the CSR performing the operation.</param>
        /// <param name="replacementPuidHigh">The PUID of the customer to whom the replacement PIN is issued.</param>
        /// <param name="replacementPuidLow">The PUID of the customer to whom the replacement PIN is issued.</param>
        /// <param name="replacementReasonCode">The reason code assigned to the replacement PIN request.</param>
        /// <param name="token">The string value of the sequence number or the token PIN.</param>
        /// <param name="tokenType">The flag that sets the token parameter to either a token identifier or token sequence number.</param>
        /// <param name="tokenSignature">The integer signature associated with the customer's original PIN.</param>
        /// <param name="emailAddress">The email address of the customer who requested the replacement PIN.</param>
        /// <param name="sessionKey">The key used for the current session.</param>
        /// <param name="trackingGuid">The GUID used for tracking this replacement transaction.</param>
        /// <param name="replacementPIN">The replacement PIN of the new token assigned to the customer.</param>
        /// <param name="signature">The signature of the new token assigned to the customer.</param>
        /// <param name="sequenceNumber">The sequence number of the new token assigned to the customer.</param>
        void GetReplacementToken(int delegatePuidHigh, int delegatePuidLow, int replacementPuidHigh, int replacementPuidLow, int replacementReasonCode, string token, int tokenType, int tokenSignature, string emailAddress, string sessionKey, string trackingGuid, out string replacementPIN, out int signature, out string sequenceNumber);
    }
}
