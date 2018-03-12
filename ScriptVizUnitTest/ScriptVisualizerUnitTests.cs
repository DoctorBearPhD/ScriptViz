using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptViz.ViewModel;

namespace ScriptVizUnitTest
{
    [TestClass]
    public class ScriptVisualizerUnitTests
    {
        [TestMethod]
        public void NextManyFramesCommand_LessThanAmountToSkipRemaining()
        {
            // Assemble
            ScriptVisualizerViewModel vm = new ScriptVisualizerViewModel
            {
                CurrentFrame = 9,
                MaxFrame = 15
            };

            // Act
            vm.NextFrameManyCommand.Execute(10);


            // Assert
            Assert.IsTrue(vm.CurrentFrame == vm.MaxFrame);
        }

        [TestMethod]
        public void NextManyFramesCommand()
        {
            // Assemble
            ScriptVisualizerViewModel vm = new ScriptVisualizerViewModel
            {
                CurrentFrame = 9,
                MaxFrame = 15
            };

            // Act
            vm.NextFrameManyCommand.Execute(2);


            // Assert
            Assert.IsTrue(vm.CurrentFrame == 11);
        }


    }
}
