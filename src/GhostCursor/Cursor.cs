using System.Geometry;
using System.Numerics;

namespace GhostCursor;

public interface ICursor
{
	Task<IAsyncDisposable> StartAsync(CancellationToken token = default);

	Task StopAsync(CancellationToken token = default);

	Task ClickAsync(string selector, int steps = 100, TimeSpan? moveSpeed = null, CancellationToken token = default);

	Task TypeAsync(string input, CancellationToken token = default);
}

public interface ICursor<in TElement> : ICursor
{
	Task ClickAsync(TElement element, int steps = 100, TimeSpan? moveSpeed = null, CancellationToken token = default);
}

public class Cursor<TBrowser, TElement> : ICursor<TElement>
	where TBrowser : IBrowser<TElement>
{
	private readonly Stopper _stopper;
	private readonly TBrowser _browser;
	private readonly Random _random;
	private readonly bool _debug;
	private bool _isStarted;
	private Vector2 _cursor;

	public Cursor(TBrowser browser, Random? random = null, bool debug = false)
	{
		_browser = browser;
		_random = random ?? ThreadRandom.Instance;
		_debug = debug;
		_stopper = new Stopper(this);
	}

	public async Task<IAsyncDisposable> StartAsync(CancellationToken token = default)
	{
		if (_isStarted)
		{
			return _stopper;
		}

		_cursor = await _browser.GetCursorAsync(token);
		_isStarted = true;
		await _browser.AllowInputAsync(false, token);

		return _stopper;
	}

	public async Task StopAsync(CancellationToken token = default)
	{
		if (!_isStarted)
		{
			return;
		}

		_isStarted = false;
		var end = await _browser.GetCursorAsync(token);
		await MoveAsync(end, token: token);
		await _browser.AllowInputAsync(true, token);
	}

	private void ValidateStarted()
	{
		if (!_isStarted)
		{
			throw new InvalidOperationException("Cursor is not started");
		}
	}

	public async Task ClickAsync(string selector, int steps = 100, TimeSpan? moveSpeed = null, CancellationToken token = default)
	{
		var element = await _browser.FindElementAsync(selector, token);
		await ClickAsync(element, steps, moveSpeed, token);
	}

	public async Task ClickAsync(TElement element, int steps = 100, TimeSpan? moveSpeed = null, CancellationToken token = default)
	{
		ValidateStarted();

		await _browser.ScrollToAsync(_cursor, _random, element, token);

		var boundingBox = await _browser.GetBoundingBox(element, token);
		var end = GetRandomPoint(boundingBox);

		await MoveAsync(end, steps, moveSpeed, token);
		await _browser.ClickAsync(end, 50, token);
	}

	private async Task MoveAsync(Vector2 end, int steps = 100, TimeSpan? moveSpeed = null, CancellationToken token = default)
	{
		var bezier = VectorUtils.BezierCurve(_random, _cursor, end);
		var moveTime = moveSpeed ?? GetMoveSpeed(_cursor, end);
		var delay = TimeSpan.FromMilliseconds(moveTime.TotalMilliseconds / steps);

		// Create debug point
		if (_debug)
		{
			await _browser.ExecuteJsAsync(
				$$"""
				  (function() {
				  	const point = document.createElement('div');
				  	point.style.position = 'fixed';
				  	point.style.width = '5px';
				  	point.style.height = '5px';
				  	point.style.borderRadius = '50%';
				  	point.style.backgroundColor = 'red'
				  	point.style.top = '{{_cursor.Y}}px';
				  	point.style.left = '{{_cursor.X}}px';
				  	point.style.zIndex = '99999999';
				  
				  	document.body.appendChild(point);
				  	
				  	window.debugPoint = point;
				  })();
				  """, token);
		}

		var currentDelay = TimeSpan.Zero;

		for (var i = 0; i < steps; i++)
		{
			if (token.IsCancellationRequested)
			{
				break;
			}

			var point = bezier.Position(i / (float)steps);
			_cursor = point;
			await _browser.MoveCursorToAsync(point, token);

			currentDelay = currentDelay.Add(delay);

			if (currentDelay.TotalMilliseconds > 5)
			{
				await Task.Delay(currentDelay, token);
				currentDelay = TimeSpan.Zero;
			}

			if (_debug)
			{
				await _browser.ExecuteJsAsync(
					$$"""
					  (function() {
					  	window.debugPoint.style.left = '{{point.X}}px';
					  	window.debugPoint.style.top = '{{point.Y}}px';
					  })();
					  """, token);
			}
		}

		await Task.Delay(_random.Next(200, 250), token);

		if (_debug)
		{
			await _browser.ExecuteJsAsync(
				"""
				(function() {
					window.debugPoint.remove();
					delete window.debugPoint;
				})();
				""", token);
		}
	}

	private TimeSpan GetMoveSpeed(Vector2 cursor, Vector2 end)
	{
		var distance = Vector2.Distance(cursor, end);

		return TimeSpan.FromMilliseconds(Math.Min(distance / 3, 200));
	}

	private Vector2 GetRandomPoint(BoundingBox boundingBox)
	{
		var x = _random.Next((int)boundingBox.Min.X + 5, (int)boundingBox.Max.X - 10);
		var y = _random.Next((int)boundingBox.Min.Y + 5, (int)boundingBox.Max.Y - 10);

		return new Vector2(x, y);
	}

	public async Task TypeAsync(string input, CancellationToken token = default)
	{
		ValidateStarted();

		await _browser.TypeAsync(_random, input, token);
		await Task.Delay(_random.Next(200, 250), token);
	}
}