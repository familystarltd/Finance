function jQueryValidatorWrapper(formId, rules, messages,msgDisplayId) {
 // hook up the form, the validation rules, and messages with jQuery validate.
    var showErrorMessage = false;
    var validator = $("#" + formId).validate({
        onchange: true,
        rules: rules,
        messages: messages,
        ignore: {},
            errorPlacement: function (error, element) {
                if (showErrorMessage) {
                    var li = document.createElement("li")
                    li.appendChild(document
                        .createTextNode(error.html()));
                    $ul.appendChild(li);
                }
            },
            showErrors: function (errorMap, errorList) {
                this.defaultShowErrors();
                if ((errorList.length != 0) && showErrorMessage) {
                }
            }
        });

    var $ul = "";
    // This is the function to call whem make the validation
    this.validate = function () {
        $("#" + msgDisplayId).empty();
        $ul = document.createElement("ul");
        $("#" + msgDisplayId).append($ul);
        showErrorMessage = true;
        var result = validator.form();
        showErrorMessage = false;
        return result;
    };
}