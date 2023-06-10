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

        const string dialogueName = "Find your way home";
        private List<DialogueOption> allOptions;
        private DialogueOption activeOption;
        private DialogueOption start;


        #region //Making the tree
        private void Awake()
        {
            allOptions = new List<DialogueOption>();
            start = new DialogueOption("Start", "You are trying to get home after getting lost in the woods. You are currently going north.");
            AddOption(start);

            //First major branch
            var west = new DialogueOption("West", "I go west instead", "You eventually find a river. The water is moving south.");
            AddOption(west, start);
            {
                var upstream = new DialogueOption("Upstream", "I follow the river upstream", "You keep walking but never find your way home.");
                AddOption(upstream, west);

                var downstream = new DialogueOption("Downstream", "I follow the river downstream", "After a couple hours, you find your way home.");
                AddOption(downstream, west);

                var across = new DialogueOption("Across", "I walk across the river", "You lose your footing and get caught in the flow.");
                AddOption(across, west);
                {
                    var accept = new DialogueOption("Accept", "Accept it and ride downstream", "After riding for awhile, it eventually leads you home.");
                    AddOption(accept, across);

                    var fight = new DialogueOption("Fight", "Fight it and climb across", "You make it across the river and continue your journey, but you never find your way home.");
                    AddOption(fight, across);
                }
            }
            
            //Second major branch
            var north = new DialogueOption("North", "I continue north", "You walk for awhile until you fall into a pit trap. There is no escape.");
            AddOption(north, start);

            //Third major branch
            var east = new DialogueOption("East", "I get east instead", "It opens to a field with wild horses.");
            AddOption(east, start);
            {
                var leave = new DialogueOption("Leave", "Leave them be and continue on my way", "You continue, but you never get home.");
                AddOption(leave, east);

                var tame = new DialogueOption("Tame", "I try to tame one", "The white horse, brown horse, or tan horse?");
                AddOption(tame, east);
                {
                    var white = new DialogueOption("White", "The white horse", "The white horse panics and kicks you in the head. You don't wake up.");
                    AddOption(white, tame);

                    var brown = new DialogueOption("Brown", "The brown horse", "You get on the brown horse, but you can't control it. It runs wildly through the woods. You don't get home.");
                    AddOption(brown, tame);

                    var tan = new DialogueOption("Tan", "The tan horse", "You successfully get on the tan horse. It calmly starts travelling south.");
                    AddOption(tan, tame);
                    {
                        var let = new DialogueOption("Accept", "Let the horse go south", "The horse leads you home.");
                        AddOption(let, tan);

                        var resist = new DialogueOption("Resist", "Resist the horse and continue eastward", "The horse submits, but you never make it home.");
                        AddOption(resist, tan);
                    }
                }

                //Last major branch
                var south = new DialogueOption("South", "I turn around and go south", "You make it home. Why were you going north?");
                AddOption(south, start);
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
