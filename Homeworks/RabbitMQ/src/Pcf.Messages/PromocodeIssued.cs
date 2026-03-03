namespace Pcf.Messages
{
    public record PromocodeIssued
    (
        Guid PartnerId,
        string BeginDate,
        string EndDate,
        Guid PreferenceId,
        string PromoCode,
        string ServiceInfo,
        Guid? PartnerManagerId = null
    );
}