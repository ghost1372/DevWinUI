// Copyright (c) Ratish Philip 
//
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions: 
// 
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software. 
// 
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE. 
//
// This file is part of the CompositionProToolkit project: 
// https://github.com/ratishphilip/CompositionProToolkit
//
// CompositionProToolkit v1.0.1
// 

namespace DevWinUI;

/// <summary>
/// Structure which holds a reference to an object used
/// within the Expression. 
/// </summary>
internal struct ExpressionParameter
{
    #region Members

    /// <summary>
    /// Reference to the object used in the Expression
    /// </summary>
    public object Reference;
    /// <summary>
    /// The type (or super-type i.e. the type from which the 
    /// object's type is derived) of the object reference.
    /// </summary>
    public Type Type;

    #endregion

    #region Construction / Initialization

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="reference">Reference to the Object used in the expression</param>
    /// <param name="type">The type (or super-type i.e. the type from which the 
    /// object's type is derived) of the object reference. </param>
    public ExpressionParameter(object reference, Type type)
    {
        Reference = reference;
        Type = type;
    }

    #endregion
}
