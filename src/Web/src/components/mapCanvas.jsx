import React from 'react';
import { Select, Input, Button, Form } from 'antd';
import { API_ENDPOINT } from '../constants';

export default class MapCanvas extends React.Component {
  defaultWidth =  300;
  defaultHeight = 300;

  constructor(props) {
    super(props);

    // Due to `draw`'s asynchronous behavior, we need to bind `this` to the
    // handler functionin order for it to be able to be used inside of `render`.
    this.handleSubmit = this.handleSubmit.bind(this);
    // Get a reference to the canvas element so we can draw on it
    this.canvasRef = React.createRef();
    this.state = {
      inputValue: '',
      searchOption: 'name',
      canvasWidth: props.width   || this.defaultWidth,
      canvasHeight: props.height || this.defaultHeight,
    };
  }

  async handleSubmit() {
    await this.draw();
  }

  // This is a lifecycle method that is called after the component has been
  // rendered to the DOM. This is where we will draw the initial state
  // on the canvas.
  componentDidMount() {
    this.draw();
  }

  async getLinePoints(endpoint = '', altKey, altVal) {
    try {
      const { searchOption, inputValue } = this.state;
      const key = altKey || this.state.searchOption;
      const val = altVal || this.state.inputValue;
      const url = `${API_ENDPOINT}/database/${endpoint}?${key}=${val}`;
      const data = await (await fetch(url)).json();

      // Combine all found sets into a singular array.
      const map = { 'id': data.length < 2 ? data[0].Id : undefined, 'points': [] };
      if (!Array.isArray(data) || data.length < 1) return map;
      data.forEach((set) => set.Objects.forEach((coord) => map.points.push(coord)));

      return map;
    } catch (ex) {
      return;
      // this.props.onError(ex);
    }
  }

  async draw() {
    // Get the 2D context from the canvas element
    const ctx = this.canvasRef.current.getContext('2d');
    // Get the points to draw from the server
    const map = await this.getLinePoints();
    const route = await this.getLinePoints('path', 'id', map ? map.id : '');

    // Set the font for the text we will draw on the canvas
    ctx.font = "24px Comic Sans MS";

    // Clear the canvas before drawing
    ctx.clearRect(0, 0, this.state.canvasWidth, this.state.canvasHeight);

    // If there are no points, display a message and return
    if (!map || map.points.length < 1) {
      ctx.fillText("No valid entry found.", 10, this.state.canvasHeight / 2);
      return;
    }

    // Loop trough all coordinates, draw a dot at each point to form a top-down
    // view of the objects.
    ctx.fillStyle = "#000";
    map.points.forEach(point => {
      ctx.beginPath();
      ctx.arc(point[1], point[0], 1, 0, 2 * Math.PI);
      ctx.fill();
    });

    ctx.fillStyle = "#800000";
    route.points.forEach(point => {
      ctx.beginPath();
      ctx.arc(point[1], point[0], 1, 0, 2 * Math.PI);
      ctx.fill();
    });
  }

  render() {
    const { searchOption, inputValue } = this.state;

    return (
      <div>
        <Form onFinish={this.handleSubmit} className='map-choice'>
          <Select
            className='map-selector'
            value={searchOption}
            onChange={(val) => this.setState({ searchOption: val })}
          >
            <Select value="name">Name</Select>
            <Select value="id">ID</Select>
            <Select value="all">All</Select>
          </Select>
          <Input
            type="text"
            value={inputValue}
            onChange={(evt) => this.setState({ inputValue: evt.target.value })}
            disabled={searchOption === 'all'}
          />
          <Button type="default" htmlType="submit">Update Map</Button>
        </Form>
        <div>
          <canvas
            ref={this.canvasRef}
            width={this.state.canvasWidth}
            height={this.state.canvasHeight}
            {...this.props}
          />
        </div>
      </div>
    );
  }
}
