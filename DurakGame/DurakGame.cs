﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Card_Lib;
using CardBox;

namespace DurakGame
{
    public partial class frmDurakGame : Form
    {
        #region VARIABLE
        private int currentCard;
        private Deck playDeck;
        private Player HumanPlayer = new Player("Player One");
        private Player ComputerPlayer = new Player("Computer");
        private Deck myDeck = new Deck(deckSize);
        private Cards discardedCards;
        private CardBox.CardBox dragCard;
        
        /// <summary>
        /// The amount, in points, that CardBox controls are enlarged when hovered over. 
        /// </summary>
        private const int POP = 25;

        /// <summary>
        /// The regular size of a CardBox control
        /// </summary>
        static private Size regularSize = new Size(250, 100);
        
        static int deckSize = 52;
        #endregion

        #region FORM AND CONTROL EVENT HANDLER
        //initialize components of the form
        public frmDurakGame()
        {
            InitializeComponent();
        }

        
        //on form load reset game
        private void frmDurakGame_Load(object sender, EventArgs e)
        {
            pbDeck.Image = (new Card()).GetCardImage();
            currentCard = 0;
            playDeck = new Deck(deckSize);
            //playDeck.LastCardDrawn += ResetGame;
            playDeck.Shuffle(deckSize);
            discardedCards = new Cards();
            txtDeckCardsRemaining.Text = playDeck.CardsRemaining.ToString();
            DealHands();
            //ResetGame();
            this.BackgroundImage = Properties.Resources.bg1;
        }

        //starts a new game
        private void btnStartGame_Click(object sender, EventArgs e)
        {
            ResetGame();

            DisplayAllCardLists();


            //AttackDefendPhase();

        }

        //on card click will raise event in card image control class
        //clicked card will go through player attack/defend phase method
        private void Card_Click(object sender, EventArgs e)
        {
           
        }


        //button pickup clicked ends human turn and picks up cards
        private void btnPickUp_Click(object sender, EventArgs e)
        {

        }

        //cease attack button ends human turn and computer starts attacking
        private void btnCeaseAttack_Click(object sender, EventArgs e)
        {


        }
        private void flpDeck_Paint(object sender, PaintEventArgs e)
        {

        }

        private void new20Deck_Click(object sender, EventArgs e)
        {
            deckSize = 20;
            // myDeck = new GameDeck(deckSize);
            //ResetGame();
        }

        private void new36Deck_Click(object sender, EventArgs e)
        {
            deckSize = 36;
            // myDeck = new GameDeck(deckSize);
            //ResetGame();
        }

        private void new52Deck_Click(object sender, EventArgs e)
        {
            deckSize = 52;
            // myDeck = new GameDeck(deckSize);
            //ResetGame();
        }

        private void lblTrumpCard_Click(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// Make the mouse pointer a "move" pointer when a drag enters a Panel.
        /// </summary>
        private void Panel_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move; // Make the mouse pointer a "move" pointer
        }

        /// <summary>
        /// Move a card/control when it is dropped from one Panel to another.
        /// </summary>
        private void Panel_DragDrop(object sender, DragEventArgs e)
        {
            // If there is a CardBox to move
            if (dragCard != null)
            {
                // Determine which Panel is which
                Panel thisPanel = sender as Panel;
                Panel fromPanel = dragCard.Parent as Panel;

                // If neither panel is null (no conversion issue)
                if (thisPanel != null && fromPanel != null)
                {
                    // If the panel are not the same
                    if (thisPanel != fromPanel)
                    {
                        // (this would happen if a card is dragged from one spot in the Panel to another)
                        // Remove the card from the Panel it started in
                        fromPanel.Controls.Remove(dragCard);
                        // Add the card to the Panel it was dropped in
                        thisPanel.Controls.Add(dragCard);
                        // Realign card in both Panels
                        RealignCards(thisPanel);
                        RealignCards(fromPanel);
                    }
                }
            }
        }
        #endregion

        #region CARDBOX EVENT HANDLER
        /// <summary>
        ///  CardBox controls grow in size when the mouse is over it.
        /// </summary>
        void CardBox_MouseEnter(object sender, EventArgs e)
        {
            // Convert sender to a CardBox
            CardBox.CardBox aCardBox = sender as CardBox.CardBox;

            // If the conversion worked
            if (aCardBox != null)
            {
                aCardBox.Size = new Size(regularSize.Width + POP, regularSize.Height + POP);// Enlarge the card for visual effect
                aCardBox.Top = 0;// move the card to the top edge of the panel.
            }
        }

        /// <summary>
        /// CardBox control shrinks to regular size when the mouse leaves.
        /// </summary>
        void CardBox_MouseLeave(object sender, EventArgs e)
        {
            // Convert sender to a CardBox
            CardBox.CardBox aCardBox = sender as CardBox.CardBox;

            // If the conversion worked
            if (aCardBox != null)
            {
                aCardBox.Size = regularSize;// resize the card back to regular size
                aCardBox.Top = POP;// move the card down to accommodate for the smaller size.

            }
        }

        /// <summary>
        /// Initiate a card move on the start of a drag.
        /// </summary>
        private void CardBox_MouseDown(object sender, MouseEventArgs e)
        {
            // Set dragCard 
            dragCard = sender as CardBox.CardBox;
            // If the conversion worked
            if (dragCard != null)
            {
                // Set the data to be dragged and the allowed effect dragging will have.
                DoDragDrop(dragCard, DragDropEffects.Move);
            }
        }

        void CardBox_Click(object sender, EventArgs e)
        {
            // Convert sender to a CardBox
            CardBox.CardBox aCardBox = sender as CardBox.CardBox;

            // If the conversion worked
            if (aCardBox != null)
            {
                // if the card is in the home panel...
                if (aCardBox.Parent == flowHumanHand)
                {
                    flowHumanHand.Controls.Remove(aCardBox); // Remove the card from the home panel
                    flowRiver.Controls.Add(aCardBox); // Add the control to the play panel
                }
                else if (aCardBox.Parent == flowComputerHand)
                {
                    flowComputerHand.Controls.Remove(aCardBox); // Remove the card from the play panel
                    flowRiver.Controls.Add(aCardBox); // Add the control to the home panel
                }
                // Realign the cards 
                //RealignCards(pnlCardHome);
                //RealignCards(pnlPlay);
            }
        }

        /// <summary>
        /// When a drag is enters a card, enter the parent panel instead.
        /// </summary>
        private void CardBox_DragEnter(object sender, DragEventArgs e)
        {
            // Convert sender to a CardBox
            CardBox.CardBox aCardBox = sender as CardBox.CardBox;

            // If the conversion worked
            if (aCardBox != null)
            {
                // Do the operation on the parent panel instead
                Panel_DragEnter(aCardBox.Parent, e);
            }
        }

        /// <summary>
        /// When a drag is dropped on a card, drop on the parent panel instead.
        /// </summary>
        private void CardBox_DragDrop(object sender, DragEventArgs e)
        {
            // Convert sender to a CardBox
            CardBox.CardBox aCardBox = sender as CardBox.CardBox;

            // If the conversion worked
            if (aCardBox != null)
            {
                // Do the operation on the parent panel instead
                Panel_DragDrop(aCardBox.Parent, e);
            }
        }
        #endregion

        #region GAME METHOD
        private void DealHands()
        {
            for (int c = 0; c < 6; c++)
            {
                HumanPlayer.PlayHand.Add(playDeck.GetCard(currentCard++));
                CardBox.CardBox aCardBox = new CardBox.CardBox(playDeck.GetCard(currentCard), true);
                aCardBox.Click += CardBox_Click;
                aCardBox.MouseDown += CardBox_MouseDown;
                aCardBox.DragEnter += CardBox_DragEnter;
                aCardBox.DragDrop += CardBox_DragDrop;
                flowHumanHand.Controls.Add(aCardBox);
            }

            for (int c = 0; c < 6; c++)
            {
                ComputerPlayer.PlayHand.Add(playDeck.GetCard(currentCard++));
                CardBox.CardBox aCardBox = new CardBox.CardBox(playDeck.GetCard(currentCard), false);
                aCardBox.Click += CardBox_Click;
                flowComputerHand.Controls.Add(aCardBox);
            }
            RealignCards(flowHumanHand);
        }
        
        //resets game
        public void ResetGame()
        {
            flowComputerHand.Controls.Clear();
            flowHumanHand.Controls.Clear();
            flowRiver.Controls.Clear();

            pbDeck.Image = (new Card()).GetCardImage();
            currentCard = 0;
            playDeck = new Deck(deckSize);
            //playDeck.LastCardDrawn += ResetGame;
            playDeck.Shuffle(deckSize);
            discardedCards = new Cards();
            txtDeckCardsRemaining.Text = playDeck.CardsRemaining.ToString();
            DealHands();
            this.BackgroundImage = Properties.Resources.bg1;
        }


        public void AttackDefendPhase()
        {
            //int userInput = 1;
            //Attacking/defendingPhase   
            //myPlayerOne.AttackingPhase(myRiver, userInput);

            //myPlayerTwo.DefendingPhase(myRiver);

            DisplayAllCardLists();

        }
        
        //will display all card lists on the windows form
        public void DisplayAllCardLists()
        {

        }

        //displays discard cards
        public void DisplayDiscardCards()
        {

        }

        //displays trump card
        public void DisplayTrumpCards()
        {

        }

        //displays player one cards
        public void DisplayPlayerOneCards()
        {

        }

        //displays player two cards
        public void DisplayPlayerTwoCards()
        {

        }

        //displays river cards
        public void DisplayRiverCards()
        {

        }
        #endregion

        #region HELPER METHOD
        private void RealignCards(Panel panelHand)
        {
            // Determine the number of cards/controls in the panel.
            int myCount = panelHand.Controls.Count;

            // If there are any cards in the panel
            if (myCount > 0)
            {
                // Determine how wide one card/control is.
                int cardWidth = panelHand.Controls[0].Width;

                // Determine where the left-hand edge of a card/control placed 
                // in the middle of the panel should be  
                int startPoint = (panelHand.Width - cardWidth) / 2;

                // An offset for the remaining cards
                int offset = 0;

                // If there are more than one cards/controls in the panel
                if (myCount > 1)
                {
                    // Determine what the offset should be for each card based on the 
                    // space available and the number of card/controls
                    offset = (panelHand.Width - cardWidth - 2 * POP) / (myCount - 1);

                    // If the offset is bigger than the card/control width, i.e. there is lots of room, 
                    // set the offset to the card width. The cards/controls will not overlap at all.
                    if (offset > cardWidth)
                        offset = cardWidth;

                    // Determine width of all the cards/controls 
                    int allCardWidth = (myCount - 1) * offset + cardWidth;

                    // Set the start point to where the left-hand edge of the "first" card should be.
                    startPoint = (panelHand.Width - allCardWidth) / 2;
                }
                // Aligning the cards: Note that I align them in reserve order from how they
                // are stored in the controls collection. This is so that cards on the left 
                // appear underneath cards to the right. This allows the user to see the rank
                // and suit more easily.

                // Align the "first" card (which is the last control in the collection)
                panelHand.Controls[myCount - 1].Top = POP;
                System.Diagnostics.Debug.Write(panelHand.Controls[myCount - 1].Top.ToString() + "\n");
                panelHand.Controls[myCount - 1].Left = startPoint;

                // for each of the remaining controls, in reverse order.
                for (int index = myCount - 2; index >= 0; index--)
                {
                    // Align the current card
                    panelHand.Controls[index].Top = POP;
                    panelHand.Controls[index].Left = panelHand.Controls[index + 1].Left + offset;

                }
            }
        }

        #endregion

        private void pbDeck_Click(object sender, EventArgs e)
        {
            // If the deck is empty (no image)
            if (pbDeck.Image == null)
            {
                // Reset the dealer
                ResetGame();
            }
            else
            {
                DealHands();
            }
        }
    }
}