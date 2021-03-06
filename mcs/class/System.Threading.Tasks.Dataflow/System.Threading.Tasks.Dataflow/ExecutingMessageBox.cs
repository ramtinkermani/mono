// ExecutingMessageBox.cs
//
// Copyright (c) 2011 Jérémie "garuma" Laval
// Copyright (c) 2012 Petr Onderka
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections.Concurrent;

namespace System.Threading.Tasks.Dataflow {
	/// <summary>
	/// Message box for executing blocks with synchrnous actions.
	/// </summary>
	/// <typeparam name="TInput">Type of the item the block is processing.</typeparam>
	class ExecutingMessageBox<TInput> : ExecutingMessageBoxBase<TInput> {
		readonly Func<bool> processItem;

		public ExecutingMessageBox (
			ITargetBlock<TInput> target, BlockingCollection<TInput> messageQueue,
			CompletionHelper compHelper, Func<bool> externalCompleteTester,
			Func<bool> processItem, Action outgoingQueueComplete,
			ExecutionDataflowBlockOptions options)
			: base (
				target, messageQueue, compHelper, externalCompleteTester,
				outgoingQueueComplete, options)
		{
			this.processItem = processItem;
		}

		/// <summary>
		/// Processes the input queue of the block.
		/// </summary>
		protected override void ProcessQueue ()
		{
			StartProcessQueue ();

			try {
				int i = 0;
				while (CanRun (i)) {
					if (!processItem ())
						break;
					i++;
				}
			} catch (Exception e) {
				CompHelper.RequestFault (e, false);
			}

			FinishProcessQueue ();
		}
	}
}