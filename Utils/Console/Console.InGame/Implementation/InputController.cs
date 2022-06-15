using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Console.InGame.Implementation
{
    internal class InputController
    {
        public float MouseScrollWheelDelta { get; private set; }

        public bool IsShift => _currentKeyboardState.IsKeyDown(Keys.LeftShift) ||
                               _currentKeyboardState.IsKeyDown(Keys.RightShift);
        public bool IsCapsLock => _currentKeyboardState.CapsLock;

        private bool IsUpperCase => IsShift && !IsCapsLock || IsCapsLock && !IsShift;
        
        private MouseState _lastMouseState;
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;

        private Keys _repeatedKey;
        private TimeSpan _repeatedKeyPressedTime;
        private static readonly TimeSpan RepeatingDelay = TimeSpan.FromMilliseconds(250);
        
        public void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            MouseScrollWheelDelta = mouseState.ScrollWheelValue - _lastMouseState.ScrollWheelValue;

            _lastMouseState = mouseState;

            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            if (_repeatedKey != Keys.None && _currentKeyboardState.GetPressedKeys().Any(key => key != _repeatedKey))
            {
                _repeatedKey = Keys.None;
            } 
        }

        public bool IsKeyPressedOnce(GameTime gameTime, Keys key)
        {
            return _currentKeyboardState[key] == KeyState.Down && _previousKeyboardState[key] == KeyState.Up;
        }

        public bool IsKeyPressedOnceRepeated(GameTime gameTime, Keys key)
        {
            if (IsKeyPressedOnce(gameTime, key))
            {
                _repeatedKeyPressedTime = TimeSpan.Zero;
                _repeatedKey = key;
                return true;
            }

            if (_currentKeyboardState.IsKeyDown(key) && _repeatedKey == key)
            {
                _repeatedKeyPressedTime += gameTime.ElapsedGameTime;
                return _repeatedKeyPressedTime >= RepeatingDelay;
            }
            
            return false;   
        }

        public char? GetPressedOnceCharacter(GameTime gameTime)
        {
            var keys = _currentKeyboardState.GetPressedKeys().Where(key => IsKeyPressedOnce(gameTime, key));
            foreach (var key in keys)
            {
                switch (key)
                {
                    case Keys.D0:
                        return IsShift ? ')' : '0';
                    case Keys.D1:
                        return IsShift ? '!' :'1';
                    case Keys.D2:
                        return IsShift ? '@' :'2';
                    case Keys.D3:
                        return IsShift ? '#' : '3';
                    case Keys.D4:
                        return IsShift ? '$' :'4';
                    case Keys.D5:
                        return IsShift ? '%' :'5';
                    case Keys.D6:
                        return IsShift ? '^' :'6';
                    case Keys.D7:
                        return IsShift ? '&' :'7';
                    case Keys.D8:
                        return IsShift ? '*' :'8';
                    case Keys.D9:
                        return IsShift ? '(' :'9';
                    case Keys.A:
                        return IsUpperCase ? 'A' :'a';
                    case Keys.B:
                        return IsUpperCase ? 'B' :'b';
                    case Keys.C:
                        return IsUpperCase ? 'C' :'c';
                    case Keys.D:
                        return IsUpperCase ? 'D' :'d';
                    case Keys.E:
                        return IsUpperCase ? 'E' :'e';
                    case Keys.F:
                        return IsUpperCase ? 'F' :'f';
                    case Keys.G:
                        return IsUpperCase ? 'G' :'g';
                    case Keys.H:
                        return IsUpperCase ? 'H' :'h';
                    case Keys.I:
                        return IsUpperCase ? 'I' :'i';
                    case Keys.J:
                        return IsUpperCase ? 'J' :'j';
                    case Keys.K:
                        return IsUpperCase ? 'K' :'k';
                    case Keys.L:
                        return IsUpperCase ? 'L' :'l';
                    case Keys.M:
                        return IsUpperCase ? 'M' :'m';
                    case Keys.N:
                        return IsUpperCase ? 'N' :'n';
                    case Keys.O:
                        return IsUpperCase ? 'O' :'o';
                    case Keys.P:
                        return IsUpperCase ? 'P' :'p';
                    case Keys.Q:
                        return IsUpperCase ? 'Q' :'q';
                    case Keys.R:
                        return IsUpperCase ? 'R' :'r';
                    case Keys.S:
                        return IsUpperCase ? 'S' :'s';
                    case Keys.T:
                        return IsUpperCase ? 'T' :'t';
                    case Keys.U:
                        return IsUpperCase ? 'U' :'u';
                    case Keys.V:
                        return IsUpperCase ? 'V' :'v';
                    case Keys.W:
                        return IsUpperCase ? 'W' :'w';
                    case Keys.X:
                        return IsUpperCase ? 'X' :'x';
                    case Keys.Y:
                        return IsUpperCase ? 'Y' :'y';
                    case Keys.Z:
                        return IsUpperCase ? 'Z' :'z';
                    case Keys.NumPad0:
                        return '0';
                    case Keys.NumPad1:
                        return '1';
                    case Keys.NumPad2:
                        return '2';
                    case Keys.NumPad3:
                        return '3';
                    case Keys.NumPad4:
                        return '4';
                    case Keys.NumPad5:
                        return '5';
                    case Keys.NumPad6:
                        return '6';
                    case Keys.NumPad7:
                        return '7';
                    case Keys.NumPad8:
                        return '8';
                    case Keys.NumPad9:
                        return '9';
                    case Keys.Multiply:
                        return '*';
                    case Keys.Subtract:
                        return '-';
                    case Keys.Decimal:
                        return IsShift ? ',' :'.';
                    case Keys.Divide:
                        return '/';
                    case Keys.OemPlus:
                        return IsShift ? '+' : '=';
                    case Keys.OemComma:
                        return IsShift ? '<' : ',';
                    case Keys.OemMinus:
                        return IsShift ? '_' : '-';
                    case Keys.OemPeriod:
                        return IsShift ? '>' : '.';
                    case Keys.OemQuestion:
                        return IsShift ? '?' : '/';
                    case Keys.OemTilde:
                        return IsShift ? '~' : '`';
                    case Keys.OemOpenBrackets:
                        return IsShift ? '{' : '[';
                    case Keys.OemCloseBrackets:
                        return IsShift ? '}' : ']';
                    case Keys.OemQuotes:
                        return IsShift ? '"' : '\'';
                    case Keys.Space:
                        return ' ';
                    case Keys.Add:
                        return '+'; 
                    case Keys.OemSemicolon:
                        return IsShift ? ':' : ';';
                    case Keys.OemPipe:
                        return IsShift ? '|' : '\\';
                    default:
                        return null;
                }
            }
            
            return null;
        }
    }
}