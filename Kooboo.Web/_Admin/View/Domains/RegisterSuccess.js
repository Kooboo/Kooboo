$(function() {
    Kooboo.Domain.getPaymentStatus({
        organizationId: self.organizationId,
        paymentId: paymentId
    }).then(function(data) {
        if (data.model.success) {
            paySuccess = true;
            clearInterval(interval);
            location.href = "/_Admin/Domains";
        }
    });
});