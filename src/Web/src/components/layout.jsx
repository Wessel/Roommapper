import React from 'react';
import { HeatMapOutlined } from '@ant-design/icons';
import { Layout, Menu, theme } from 'antd';

import Controlfield from './controlField';
import MapCanvas from './mapCanvas';

const { Content, Sider } = Layout;

// Sidebar menu items
const menuItems = [
  {
    key: 'map',
    icon: React.createElement(HeatMapOutlined),
    label: 'Map',
  }
];

// I.v.m. het gebruik van de "antd" UI library moet er gebruik gemaakt worden
// van een "hook", deze react onderdelen kunnen niet gebruikt worden in een
// class en dus is dit het enigste niet-OOP gedeelte van de frontend.
const App = () => {
  const { token: { colorBgContainer, borderRadiusLG } } = theme.useToken();
  return (
    <Layout hasSider>
      <Sider
        style={{
          overflow: 'auto',
          height: '100vh',
          position: 'fixed',
          left: 0,
          top: 0,
          bottom: 0,
        }}
        theme='dark'
      >
        <Menu
          theme="dark"
          mode="inline"
          defaultSelectedKeys={['map']}
          items={menuItems}
        />
      </Sider>
      <Layout
        theme='dark'
        style={{ marginLeft: 200 }}
      >
        <Content
          style={{ margin: '24px 16px 0', overflow: 'initial' }}
        >
          <div
            style={{
              padding: 24,
              textAlign: 'left',
              background: colorBgContainer,
              borderRadius: borderRadiusLG,
            }}
          >
            <div style={{ display: 'flex' }}>
              <Controlfield />
              <div style={{ 'marginLeft': '50px' }}>
                <MapCanvas
                  width={500}
                  height={500}
                  onError={Controlfield.handleError}
                  className='map-canvas'
                />
              </div>
            </div>
          </div>
        </Content>
      </Layout>
    </Layout>
  );
};

export default App;
