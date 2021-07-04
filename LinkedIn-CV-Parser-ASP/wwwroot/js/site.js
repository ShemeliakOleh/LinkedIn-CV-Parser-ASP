var limit;
if ($('input[class="core-stack-checkbox"]').length !== 0) {
    $.get("https://linkedincvparser.azurewebsites.net/Preview/CoreStackLimit", data => limit = data);
}
$('input[class="core-stack-checkbox"]').on('change', function (evt) {
    if ($('input[class="core-stack-checkbox"]:checked').length > limit) {
       this.checked = false;
   }
});
function ToggleCoreStackCheckBox(elem) {
    if (elem.checked == true) {
        $(elem).siblings('.core-stack-checkbox')[0].disabled = false;
    } else {
        $(elem).siblings('.core-stack-checkbox')[0].checked = false;
        $(elem).siblings('.core-stack-checkbox')[0].disabled = true;
    }
}