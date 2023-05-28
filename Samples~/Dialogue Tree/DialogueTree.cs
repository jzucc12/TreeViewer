using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// Dialogue tree object. Tree viewable in play mode only.
    /// </summary>
    public class DialogueTree : MonoBehaviour, ITreeViewer
    {
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private Text bannerText;
        [SerializeField] private Button optionButtonPrefab;

        const string dialogueName = "Product Review \"Survey\"";
        private List<DialogueOption> allOptions;
        private DialogueOption activeOption;
        private DialogueOption start;


        #region //Making the tree
        private void Awake()
        {
            allOptions = new List<DialogueOption>();
            start = new DialogueOption("Start", "What do you think of this package so far?");
            AddOption(start);

            //First major branch
            var amazing = new DialogueOption("Amazing", "Amazing, I love it!", "Happy to hear it! What's your favorite part?");
            AddOption(amazing, start);
            {
                var all = new DialogueOption("All", "All of it. So great!", "That's great! Well, have a good one!");
                AddOption(all, amazing);

                var convenience = new DialogueOption("Convenience", "The convenience it provides.", "Well that was the point. So good to hear.");
                AddOption(convenience, amazing);
                {
                    var pointMade = new DialogueOption("Made", "Point well made then.", "Thanks. Happy using!");
                    AddOption(pointMade, convenience);
                }


                var samples = new DialogueOption("Samples", "The samples obviously.","....Which one is your favorite?");
                AddOption(samples, amazing);
                {
                    var ui = new DialogueOption("UI", "The UI one.", "That one? Really? We're done here.");
                    AddOption(ui, samples);

                    var hsm = new DialogueOption("HSM", "The hierarchal state machine.", "Of course you'd pick that one... Get outta here!");
                    AddOption(hsm, samples);

                    var thisOne = new DialogueOption("This one", "This one of course!", "Aw shucks. Don't tell the others, but you're my favorite user");
                    AddOption(thisOne,samples);
                    {
                        var kind = new DialogueOption("Kind", "You're too kind. I bet you say that to all the users.", "If they go down this path, yes I do.");
                        AddOption(kind, thisOne);
                    }
                }
            }
            
            //Second major branch
            var okay = new DialogueOption("Okay", "It's okay.", "I guess I'll that over bad. Well bye then.");
            AddOption(okay, start);

            //Last major branch
            var notAFan = new DialogueOption("Not a fan", "Can't say I'm a fan.", "...It's not the samples, is it?");
            AddOption(notAFan, start);
            {
                var no = new DialogueOption("Not samples", "No, not them. It's just not for me.", "That was close... Fair enough though. Thanks for the honesty.");
                AddOption(no, notAFan);

                var yes = new DialogueOption("Yes", "...It is","...Is it me?");
                AddOption(yes, notAFan);
                {
                    var ui = new DialogueOption("UI", "No, it's the UI one", "Yeah! Take that UI one!");
                    AddOption(ui, yes);

                    var hsm = new DialogueOption("HSM", "It's the hierarchal state machine actually", "I Wasn't expecting that, but I'll take it.");
                    AddOption(hsm, yes);

                    var itsYou = new DialogueOption("It's you", "Yes, it's you...", ":(");
                    AddOption(itsYou,yes);
                }
            }

            Reset();
        }

        private void AddOption(DialogueOption newOption, DialogueOption parent = null)
        {
            allOptions.Add(newOption);
            parent?.AddChild(newOption);
        }
        #endregion

        #region //Dialogue selection
        private void ChooseOption(DialogueOption option)
        {
            //Enter state
            activeOption = option;
            option.EnterOption();
            bannerText.text = option.bannerText;

            //Destroy old buttons
            foreach(Transform child in buttonContainer)
            {
                Destroy(child.gameObject);
            }
            buttonContainer.DetachChildren();

            //Make new buttons
            foreach(DialogueOption child in option.children)
            {
                Button newButton = Instantiate(optionButtonPrefab, buttonContainer);
                newButton.GetComponentInChildren<Text>().text = child.playerResponse;
                newButton.onClick.AddListener(() => ChooseOption(child));
            }

            //Show end button if current has no children
            if(buttonContainer.childCount == 0)
            {
                Button end = Instantiate(optionButtonPrefab, buttonContainer);
                end.GetComponentInChildren<Text>().text = "The end. Press to restart.";
                end.onClick.AddListener(Reset);
            }
        }

        private void Reset()
        {
            start.ExitOption();
            ChooseOption(start);
        }
        #endregion

        #region //Interface specific
        public IEnumerable<ITreeNodeViewer> GetAllNodes()
        {
            return allOptions;
        }

        public string GetTreeName()
        {
            return dialogueName;
        }
        #endregion
    }
}
