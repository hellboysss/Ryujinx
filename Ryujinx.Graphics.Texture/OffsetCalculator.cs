using Ryujinx.Common;
using System.Runtime.CompilerServices;
using static Ryujinx.Graphics.Texture.BlockLinearConstants;

namespace Ryujinx.Graphics.Texture
{
    public class OffsetCalculator
    {
        private int  _width;
        private int  _height;
        private int  _stride;
        private bool _isLinear;
        private int  _bytesPerPixel;

        private BlockLinearLayout _layoutConverter;

        // Variables for built in iteration.
        private int _yPart;

        public OffsetCalculator(
            int  width,
            int  height,
            int  stride,
            bool isLinear,
            int  gobBlocksInY,
            int  bytesPerPixel)
        {
            _width         = width;
            _height        = height;
            _stride        = stride;
            _isLinear      = isLinear;
            _bytesPerPixel = bytesPerPixel;

            int wAlignment = GobStride / bytesPerPixel;

            int wAligned = BitUtils.AlignUp(width, wAlignment);

            if (!isLinear)
            {
                _layoutConverter = new BlockLinearLayout(
                    wAligned,
                    height,
                    1,
                    gobBlocksInY,
                    1,
                    bytesPerPixel);
            }
        }

        public void SetY(int y)
        {
            if (_isLinear)
            {
                _yPart = y * _stride;
            }
            else
            {
                _layoutConverter.SetY(y);
            }
        }

        public int GetOffset(int x, int y)
        {
            if (_isLinear)
            {
                return x * _bytesPerPixel + y * _stride;
            }
            else
            {
                return _layoutConverter.GetOffset(x, y, 0);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetOffset(int x)
        {
            if (_isLinear)
            {
                return x * _bytesPerPixel + _yPart;
            }
            else
            {
                return _layoutConverter.GetOffset(x);
            }
        }

        public (int offset, int size) GetRectangleRange(int x, int y, int width, int height)
        {
            if (_isLinear)
            {
                return (y * _stride + x * _bytesPerPixel, height * _stride);
            }
            else
            {
                return _layoutConverter.GetRectangleRange(x, y, width, height);
            }
        }

        public bool LayoutMatches(OffsetCalculator other)
        {
            if (_isLinear)
            {
                return other._isLinear &&
                       _width == other._width &&
                       _height == other._height &&
                       _stride == other._stride &&
                       _bytesPerPixel == other._bytesPerPixel;
            }
            else
            {
                return !other._isLinear && _layoutConverter.LayoutMatches(other._layoutConverter);
            }
        }
    }
}