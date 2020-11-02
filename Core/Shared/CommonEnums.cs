using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace AppZeroAPI.Shared.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CardPaymentStatus
    {
        AUTHORIZED,
        AUTHORIZING,
        SETTLED,
        SETTLING,
        SETTLEMENT_CONFIRMED,
        SETTLEMENT_PENDING,
        SUBMITTED_FOR_SETTLEMENT
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AddressType
    {
        Billing,
        Shipping
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PaymentMethod
    {
        Cash,
        CreditCrad,
        Bank
    }
    public enum ShippingStatus
    {
        Draft,
        Planned,
        Shipped
    }

    public enum ShippingType
    {
        None,
        Mail,
        Digital
    }

    public enum PaymentProviderType
    {
        None,
        StripeCheckout,
        StripeElements,
        StripeBilling,
        Vipps
    }
    public enum OrderStatus
    {
        Draft,
        Verified,
        Invoiced,
        Paid,
        Cancelled,
        Refunded
    }
    public enum SiteUserLevelType
    {
        [Description("AdminUser")]
        AdminUser = 1,
        [Description("User")]
        User = 2
    }

    public enum PaymentType
    {
        [Description("Cash")]
        Cash = 1,
        [Description("Check")]
        Check = 2,
        [Description("CreditCard")]
        CreditCard = 3,
    }

    public enum PaymentMethodType
    {
        [Description("Visa")]
        Visa = 1,
        [Description("MasterCard")]
        MasterCard = 2,
        [Description("American Express")]
        AmericanExpress = 3,
        [Description("Cash")]
        Cash = 11,
        [Description("Check")]
        Check = 12,
    }

    public enum PaymentStatusType
    {
        [Description("Purchase")]
        Purchase = 1,
        [Description("Void")]
        Void = 2,
        [Description("Refund")]
        Refund = 3,
    }

    public enum PaymentGateWayType
    {
        [Description("HelCim")]
        HelCim = 1,
        [Description("Stripe")]
        Stripe = 2
    }

    public enum CurrencyType
    {
        [Description("USD")]
        USD = 1,
        [Description("CAD")]
        CAD = 2
    }

    public enum IndexNameType
    {
        [Description("patient-index")]
        Patient = 1,
        [Description("invoice-index")]
        Invoice = 2,
        [Description("service-index")]
        Service = 3
    }
 
   public enum TemplateTypes
    {
        ConfirmationEmail = 1,
        ActivationEmail = 2,
        InviteEmail = 3,
        InviteEmailWithoutLink = 4,
        ShareRefferalLink = 5,
        SendEmailOnsubmit = 6,
        ForgotPassword = 7,
        NotificationEmail = 8,
        PaymentConfirmationEmail = 9,
        UserAddedToWorkflow = 10,
        UserDeletedFromWorkFlow = 11,
        FormPaymentConfirmationEmail = 12,
        ChangeUserEmail = 13,
        BlazeFormDeleteOrganization = 14,
        BlazeFormActivationOrganization = 15,
        BlazeFormDeActivationOrganization = 16,
        ContactEmail = 17,
        ResetPasswordURLEmail = 18,
        EmailToSuperAdmin = 19,
        ResetPassword=20
    }
    public enum SystemRoles
    {
        Administrator = 1
    }
    public enum BActions
    {
        Archive = 1, Delete = 2, Move = 3
    }
    public enum PlanType
    {
        Def = 1, Show = 2
    }
    public enum EntriesStatus
    {
        Submitted = 1,
        Reviewed = 2,
        Complete = 3,
        Incomplete = 4
    }
    public enum planInterval
    {
        Monthly = 30,
        quarterly = 90,
        HalfYearly = 180,
        Yearly = 365

    }
    public enum PaymentStatus
    {
        Paid = 1,
        Failure = 2,
        Skipped = 3,
        Cancel = 4,
        Cash = 5
    }
    public enum FormType
    {
        Form = 0,
        WorkFlow = 1
    }
    public enum WorkFlowUserType
    {
        Owner = 0,
        Participant = 1,
        Removed = 2
    }
    public enum ActivatedUserType
    {
        //NewUser=1,
        //InvitedNewUser=2,
        //InvitedUser=3,
        Invited = 1,
        FreePlan = 2,
        PaidPlan = 3
    }
    public enum FormPermissionType
    {

        View = 1,
        Delete,
        Edit
    }

}
