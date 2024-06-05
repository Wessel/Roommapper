import React from 'react';
import { Button, Card } from 'antd';

import { API_ENDPOINT } from '../constants';

// Crude way of making an enum.
const tasks = Object.freeze({
  START: 0,
  STOP: 1,
  MAP: 2,
});

export default class ControlField extends React.Component {
  constructor(props) {
    super(props);
    // Save the state of the "message" field.
    this.state = {
      result: null
    };
  }

  handleError(error) {
    // Upload error messages to the "Output Message" field.
    this.setState({ result: error.message });
  };

  handleClick(task) {
    // Parse the pressed button and send the corresponding task to the server.
    task = task === tasks.START ? 'start' : task === tasks.STOP ? 'stop' : 'map';
    fetch(`${API_ENDPOINT}/roomba/control`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ task: task })
    })
      .then(response => response.json())
      .then(data => this.setState({ result: JSON.stringify(data) }))
      .catch(this.handleError);
  }

  render() {
    return (
      <div className='control'>
        <div className='control-buttons'>
          <Button type='primary' onClick={() => this.handleClick(tasks.START)}>Start</Button>
          <Button type='primary' onClick={() => this.handleClick(tasks.MAP)}>Map</Button>
          <Button type='primary' danger onClick={() => this.handleClick(tasks.STOP)}>Stop</Button>
        </div>
        <Card
          className='output-field'
          title="Output Message"
          bordered={false}
        >
          <pre>{this.state.result}</pre>
        </Card>
      </div>
    );
  }
}
