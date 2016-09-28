define(["sitecore", "/-/speak/v1/ExperienceEditor/ExperienceEditor.js"], function (Sitecore, ExperienceEditor) {
    Sitecore.Commands.ShowPlaygroundNavigationBar =
    {
        strip: "PlaygroundStrip",
        canExecute: function (context) {
            return true;
        },
        execute: function (context) {
            //execute command
        }
    };
});