define(["sitecore", "/-/speak/v1/ExperienceEditor/ExperienceEditor.js"], function (Sitecore, ExperienceEditor) {
    return {
        priority: 1,
        execute: function (context) {
            var shouldHide = true; // decision
            if (shouldHide) {
                var playgroundstrip = "PlaygroundStrip";
                context.commands = context.commands.filter(function (e) {
                    return e.stripId !== playgroundstrip;
                });

                context.commands = context.commands.filter(function (e) {
                    if (e.command && e.command.strip && e.command.strip === playgroundstrip) {
                        return false;
                    }
                    return true;
                });

                var experienceAcceleratorStrip = document.querySelector('[stripid=' + playgroundstrip + ']');
                if (experienceAcceleratorStrip) {
                    experienceAcceleratorStrip.hide();
                }
            }
        }
    };
});